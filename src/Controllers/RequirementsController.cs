using System.Collections.Generic;
using System;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileReader = System.IO.File;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Models.Profile;

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
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var parentRequirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == parentId);
                if (parentRequirement == null)
                    return NotFound();

                string profile = null;

                if (parentRequirement.Requirements.Count == 0 && parentRequirement.Parent != null)
                {
                    profile = parentRequirement.Profile;
                    parentRequirement.Profile = null;
                    db.Requirements.Update(parentRequirement);
                    await db.SaveChangesAsync();
                }
                else
                {
                    profile = await FileReader.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/profile.json");
                    
                    var project = GetRoot(parentRequirement);

                    string json = project.Profile;

                    var projectProfile = JsonConvert.DeserializeObject<List<SoftwareRequirements.Models.Profile.Profile>>(json);
                    var index = projectProfile.FirstOrDefault(i => i.NameIndex == "I8");
                    if (index != null)
                    {
                        var coeff = index.Coefficients.LastOrDefault();
                        string kIndex = null;
                        if (coeff != null)
                        {
                            int firstIndexK = coeff.Name.IndexOf("K") + 1;
                            int value = int.Parse(coeff.Name.Substring(firstIndexK)) + 1;

                            kIndex = "K" + value;
                        }
                        else
                        {
                            kIndex = "K1";
                        }
                        index.Coefficients.Add(new Coefficient
                        {
                            Name = kIndex,
                            Value = null
                        });
                    }
                    string updateProfile = JsonConvert.SerializeObject(projectProfile);

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

                return Created($"/profiles/{newRequirement.Id}", newRequirement.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirementById(int id)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);
            if (requirement == null)
                return NotFound();

            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var project = GetRoot(requirement);
                // TO DO: Remove coeff from profile of a project

                RemoveChildren(requirement);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateNameRequirement(int id, [FromBody]RequirementCreate requirementChange)
        {
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent != null);
                if (requirement == null)
                    return NotFound();
                requirement.Name = requirementChange.Name;

                db.Requirements.Update(requirement);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500);
            }
            return NoContent();
        }

        [HttpGet("{id}/Coefficients/{coeffId}")]
        public async Task<IActionResult> GetResult(int id, string coeffId)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent != null);
            if (requirement == null)
                return NotFound();

            if (coeffId != "I9")
            {
                
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

        private Requirement GetRoot(Requirement requirement)
        {
            if (requirement.Parent == null)
                return requirement;
            return GetRoot(requirement.Parent);
        }
    }
}