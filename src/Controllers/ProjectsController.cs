using System;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FileReader = System.IO.File;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;

namespace SoftwareRequirements.Controllers
{
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationContext db;
        private readonly IMapper mapper;

        public ProjectsController(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody]RequirementCreate project)
        {
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var newProject = new Requirement()
                    {
                        Name = project.Name,
                        Profile = await FileReader.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/baseProfile.json")
                    };

                    await db.Requirements.AddAsync(newProject);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Created($"/profiles/{newProject.Id}", newProject.Id);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects([FromQuery]int offset, [FromQuery]int size)
        {
            if (size > 100)
                return BadRequest();

            var projects = await db.Requirements.Where(r => r.Parent == null).Skip(offset).Take(size).ToListAsync();

            if (projects.Count == 0)
                return NotFound();

            var projectList = mapper.Map<List<Requirement>, List<RequirementListView>>(projects);
            return Ok(projectList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectsById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);

            if (project == null)
                return NotFound();

            var requirementView = mapper.Map<Requirement, RequirementView>(project);
            return Ok(requirementView);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateNameProject(int id, [FromBody]RequirementCreate projectChange)
        {
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);
                    if (project == null)
                        return NotFound();
                    project.Name = projectChange.Name;

                    db.Requirements.Update(project);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500);
                }
            }
            return NoContent();
        }

        [HttpGet("{id}/Profiles")]
        public async Task<IActionResult> GetAllProfilesById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);
            if (project == null)
                return NotFound();
            
            var profileListView = mapper.Map<Requirement, ProfileListView>(project);
            return Ok(profileListView);
        }
    }
}