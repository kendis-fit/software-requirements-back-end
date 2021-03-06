using System.IO;
using AutoMapper;
using System.Linq;
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
    public class ProjectRepository : 
        ICalculableProfileRepository,
        ISearchableRepository<Task<RequirementView>, int>,
        ISelectableRepository<Task<List<RequirementListView>>>,
        ICreatableRepository<Task<Requirement>, ProjectCreate>
    {
        private readonly IMapper mapper;
        private readonly ApplicationContext db;

        public ProjectRepository(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Requirement> Create(ProjectCreate project)
        {
            var newProject = new Requirement();

            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                newProject = new Requirement()
                {
                    Name = project.Name,
                    Profile = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/baseProfile.json"),
                    Write = RequirementWrite.CREATED
                };

                await db.Requirements.AddAsync(newProject);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new ServerErrorException("DB Failed");
            }
            return newProject;
        }

        public async Task<float> Calculate(int id, string indexId)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);
            if (project == null)
            {
                throw new NotFoundException("Project not found");
            }

            var projectProfileResult = Convert(project, indexId);

            float result = new CalculateProfile(projectProfileResult).Calculate();
            return result;
        }

        public async Task<List<ProfileRadarResult>> ConvertToDiagram(int id, string indexId)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);
            if (project == null)
            {
                throw new NotFoundException("Project not found");
            }
            SortInDepth(ref project);
            var projectProfileResult = Convert(project, indexId);

            var radarResults = new List<ProfileRadarResult>();

            var connector = new BaseConnectorProfile().MakeConnect();

            var isMetric = projectProfileResult.ProfileResults.FirstOrDefault().Name.Contains("K");
            
            foreach (var profileResult in projectProfileResult.ProfileResults)
            {
                string profileResultName = isMetric ? 
                    connector[indexId].FirstOrDefault(coeff => coeff.Coefficient == profileResult.Name).Index 
                    : profileResult.Name;

                float? value = !isMetric ? profileResult.Coeff : profileResult.Value;

                string name = $"{profileResultName} ({profileResult.Coeff})";
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

        public async Task<List<RequirementListView>> GetAll(int offset, int size)
        {
            if (size > 100)
            {
                throw new BadRequestException("Size is more than 100");
            }

            var projects = await db.Requirements.Where(r => r.Parent == null).Skip(offset).Take(size).ToListAsync();
            if (projects.Count == 0)
            {
                throw new NotFoundException("Projects are empty");
            }

            var projectList = mapper.Map<List<Requirement>, List<RequirementListView>>(projects);
            return projectList;
        }

        public async Task<RequirementView> FindById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);

            if (project == null)
            {
                throw new NotFoundException("Project not found");
            }
            SortInDepth(ref project);

            var requirementView = mapper.Map<Requirement, RequirementView>(project);
            return requirementView;
        }

        private void SortInDepth(ref Requirement view)
        {
            if (view.Requirements != null && view.Requirements.Count() != 0)
            {
                view.Requirements = view.Requirements.OrderBy(r => r.Id).ToList();
                foreach (var item in view.Requirements)
                {
                    var test = item;
                    SortInDepth(ref test);
                }
            }
        }
    }
}