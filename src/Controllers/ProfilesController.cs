using AutoMapper;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;

namespace SoftwareRequirements.Controllers
{
    [Route("[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationContext db;
        private readonly IMapper mapper;

        public ProfilesController(ApplicationContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);

            if (profile == null)
                return NotFound();
            
            var profileView = mapper.Map<Requirement, ProfileView>(profile);
            return Ok(profileView.Profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfileById(int id, [FromBody]string profile)
        {
            var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id);

            if (requirement == null)
                return NotFound();

            using var transaction = await db.Database.BeginTransactionAsync();
            
            try
            {
                requirement.Profile = profile;
                requirement.Write = RequirementWrite.DONE;
                db.Requirements.Update(requirement);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500);
            }
            return Ok();
        }
    }
}