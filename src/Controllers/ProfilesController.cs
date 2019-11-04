using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
    }
}