using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SoftwareRequirements.Exceptions;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Repositories;

namespace SoftwareRequirements.Controllers
{
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectRepository repository;

        public ProjectsController(ProjectRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody]ProjectCreate project)
        {
            try
            {
                var newProject = await repository.Create(project);
                return Created($"/profiles/{newProject.Id}", newProject.Id);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects([FromQuery]int offset, [FromQuery]int size)
        {
            try
            {
                var projects = await repository.GetAll(offset, size);
                return Ok(projects);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectsById(int id)
        {
            try
            {
                var project = await repository.FindById(id);
                return Ok(project);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        // [HttpPatch("{id}")]
        // public async Task<IActionResult> UpdateNameProject(int id, [FromBody]RequirementCreate projectChange)
        // {
        //     using var transaction = await db.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);
        //         if (project == null)
        //             return NotFound();
        //         project.Name = projectChange.Name;

        //         db.Requirements.Update(project);
        //         await db.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //     }
        //     catch
        //     {
        //         await transaction.RollbackAsync();
        //         return StatusCode(500);
        //     }
        //     return NoContent();
        // }

        [HttpGet("{id}/Indexes/{indexId}")]
        public async Task<IActionResult> CalculateResult(int id, string indexId)
        {
            try
            {
                float result = await repository.Calculate(id, indexId);
                return Ok(result);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        [HttpGet("{id}/Diagrams/{indexId}")]
        public async Task<IActionResult> CalculateDatasetDiagram(int id, string indexId)
        {
            try
            {
                var result = await repository.ConvertToDiagram(id, indexId);
                return Ok(result);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }
    }
}