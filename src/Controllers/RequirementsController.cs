using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileReader = System.IO.File;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;

namespace SoftwareRequirements.Controllers
{
    [Route("[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly ApplicationContext db;
        private readonly IMapper mapper;

        public RequirementsController(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpPost("{parentId}")]
        public async Task<IActionResult> CreateRequirements(int parentId, [FromBody]RequirementCreate requirement)
        {
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var parentRequirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == parentId);
                    if (parentRequirement == null)
                        return NotFound();

                    var newRequirement = new Requirement()
                    {
                        Name = requirement.Name,
                        Parent = parentRequirement,
                        Profile =  await FileReader.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/profile.json")
                    };

                    await db.Requirements.AddAsync(newRequirement);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Created($"/profiles/{newRequirement.Id}", newRequirement.Id);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirementById(int id)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent != null);
            if (requirement == null)
                return NotFound();

            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    RemoveChildren(requirement);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500);
                }
            }
            return Ok();
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
    }
}