using System;
using AutoMapper;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Repositories.Interfaces;

namespace SoftwareRequirements.Repositories
{
    public class ProjectRepository : ISearchableRepository<Task<RequirementView>, int>, ICreatableRepository<RequirementCreate>
    {
        private readonly IMapper mapper;
        private readonly ApplicationContext db;

        public ProjectRepository(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<RequirementView> FindById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);

            if (project == null)
            {
                return null;
            }

            var requirementView = mapper.Map<Requirement, RequirementView>(project);
            return requirementView;
        }

        public async void Create(RequirementCreate project)
        {
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var newProject = new Requirement()
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
                throw new Exception("");
            }
        } 
    }
}