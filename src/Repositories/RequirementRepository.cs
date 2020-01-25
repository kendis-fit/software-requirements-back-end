using System;
using System.IO;
using AutoMapper;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Exceptions;
using SoftwareRequirements.Helpers.Converter;
using SoftwareRequirements.Helpers.Algorithm;
using SoftwareRequirements.Repositories.Interfaces;
using SoftwareRequirements.Helpers.Converter.Connections;
using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Repositories
{
    public class RequirementRepository :
        ICalculableProfileRepository,
        ICreatableRepository<Task<Requirement>,RequirementCreate>
    {
        private readonly IMapper mapper;
        private readonly ApplicationContext db;

        public RequirementRepository(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Requirement> Create(RequirementCreate requirement)
        {
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var parentRequirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == requirement.ParentId);
                if (parentRequirement == null)
                {
                    throw new NotFoundException("Requirement not found");
                }

                string profile = null;

                if (IsGroup(parentRequirement))
                {
                    profile = parentRequirement.Profile;
                    parentRequirement.Profile = null;
                    db.Requirements.Update(parentRequirement);
                    await db.SaveChangesAsync();
                }
                else
                {
                    profile = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/profile.json");
                    
                    var project = GetRoot(parentRequirement);

                    var json = JsonConvert.DeserializeObject<List<Models.Profile.Profile>>(project.Profile);

                    var profileRepository = new ProfileRepository(json).AddCoeffToI8();

                    string updateProfile = JsonConvert.SerializeObject(profileRepository);

                    project.Profile = updateProfile;

                    db.Requirements.Update(project);
                    await db.SaveChangesAsync();
                }

                var newRequirement = new Requirement()
                {
                    Name = requirement.Name,
                    Parent = parentRequirement,
                    Profile = profile
                };

                await db.Requirements.AddAsync(newRequirement);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return newRequirement;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ServerErrorException(ex.Message);
            }
        }

        public async Task Remove(int id)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);
            if (requirement == null)
            {
                throw new NotFoundException($"Requirement {id} not found");
            }
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                if (requirement.ParentId == null)
                {
                    RemoveChildren(requirement);
                }
                else
                {
                    var project = GetRoot(requirement);

                    int countRemoved = 0;
                    RemoveChildren(requirement, ref countRemoved);

                    var projectProfile = JsonConvert.DeserializeObject<List<SoftwareRequirements.Models.Profile.Profile>>(project.Profile);                
        
                    var profileRepository = new ProfileRepository(projectProfile).RemoveLastCoeffsI8(countRemoved);                    

                    project.Profile = JsonConvert.SerializeObject(profileRepository);
                    db.Requirements.Update(project);
                    await db.SaveChangesAsync();

                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ServerErrorException(ex.Message);
            }
        }

        private bool IsGroup(Requirement requirement) => requirement.Requirements.Count == 0 && requirement.Parent != null;

        private Requirement GetRoot(Requirement requirement)
        {
            if (requirement.Parent == null)
                return requirement;
            return GetRoot(requirement.Parent);
        }

        private void RemoveChildren(Requirement requirement)
        {
            if (requirement.Requirements.Count > 0)
            {
                foreach (var child in requirement.Requirements.ToList())
                {
                    RemoveChildren(child);
                }
            }
            db.Requirements.Remove(requirement);
            db.SaveChanges();
        }

        private void RemoveChildren(Requirement requirement, ref int countRemoved)
        {
            bool isGroup() => string.IsNullOrEmpty(requirement.Profile);

            if (requirement.Requirements.Count > 0)
            {
                foreach (var child in requirement.Requirements.ToList())
                {
                    RemoveChildren(child, ref countRemoved);
                }
            }
            if (!isGroup())
            {
                ++countRemoved;
            }
            db.Requirements.Remove(requirement);
            db.SaveChanges();
        }

        public async Task<float> Calculate(int id, string indexId)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent != null);
            if (requirement == null)
            {
                throw new NotFoundException($"Requirement {id} not found");
            }
            var projectProfileResult = Convert(requirement, indexId);

            float result = new CalculateProfile(projectProfileResult).Calculate();
            return result;
        }

        public async Task<List<ProfileRadarResult>> ConvertToDiagram(int id, string indexId)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);
            if (project == null)
            {
                throw new NotFoundException($"Requirement {id} not found");
            }
            var projectProfileResult = Convert(project, indexId);

            var radarResults = new List<ProfileRadarResult>();
            
            var connector = new ConnectorProfile().MakeConnect();

            var isMetric = projectProfileResult.ProfileResults.FirstOrDefault().Name.Contains("K");

            foreach (var profileResult in projectProfileResult.ProfileResults)
            {
                string profileResultName = isMetric ? 
                    connector[indexId].FirstOrDefault(coeff => coeff.Coefficient == profileResult.Name).Index 
                    : profileResult.Name;

                float? value = !isMetric ? profileResult.Coeff : profileResult.Value;

                string name = $"{profileResultName} ({value})";
                float result = new CalculateProfile(profileResult).Calculate();
                radarResults.Add(new ProfileRadarResult { Name = name, Value = result });
            }
            return radarResults;
        }

        private ProfileResult Convert(Requirement project, string indexId)
        {
            var profileListView = mapper.Map<Requirement, ProfileListView>(project);

            var profileConverter = new ProfileConverter(profileListView, indexId);
            var projectProfileResult = profileConverter.Convert();

            return projectProfileResult;
        }
    }
}