using System;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Helpers.Converter;
using SoftwareRequirements.Helpers.Algorithm;
using SoftwareRequirements.Repositories.Interfaces;
using System.Collections.Generic;

namespace SoftwareRequirements.Repositories
{
    public class ProjectRepository : 
        ICalculableProfileRepository,
        ISearchableRepository<Task<Requirement>, int>,
        ISelectableRepository<Task<List<RequirementListView>>>,
        ICreatableRepository<Task<Requirement>,RequirementCreate>
    {
        private readonly IMapper mapper;
        private readonly ApplicationContext db;

        public ProjectRepository(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Requirement> Create(RequirementCreate project)
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
                throw new Exception("DB Failed");
            }
            return newProject;
        }

        public float Calculate(Requirement project, string indexId)
        {
            var profileListView = mapper.Map<Requirement, ProfileListView>(project);

            var profileConverter = new ProfileConverter(profileListView, indexId);
            var projectProfileResult = profileConverter.Convert(); 

            float result = new CalculateProfile(projectProfileResult).Calculate();
            return result;
        }

        public async Task<List<RequirementListView>> GetAll(int offset, int size)
        {
            var projects = await db.Requirements.Where(r => r.Parent == null).Skip(offset).Take(size).ToListAsync();

            var projectList = mapper.Map<List<Requirement>, List<RequirementListView>>(projects);
            return projectList;
        }

        public async Task<Requirement> FindById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);

            if (project == null)
            {
                return null;
            }
            return project;
        }
    }
}