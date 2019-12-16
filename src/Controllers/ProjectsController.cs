using System.Runtime.InteropServices;
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
using SoftwareRequirements.Models;
using SoftwareRequirements.Profiles;
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
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var newProject = new Requirement()
                {
                    Name = project.Name,
                    Profile = await FileReader.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Json/baseProfile.json"),
                    Write = RequirementWrite.CREATED
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
            using var transaction = await db.Database.BeginTransactionAsync();
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
            return NoContent();
        }

        [HttpGet("{id}/Profiles")]
        public async Task<IActionResult> GetAllProfilesById(int id)
        {
            var project = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent == null);
            if (project == null)
                return NotFound();
            
            var profileListView = mapper.Map<Requirement, ProfileListView>(project);

            int countI8 = 0;
            CountI8(profileListView, ref countI8);

            List<ConnectResult> I8 = new List<ConnectResult>();
            for (int i = 0; i < countI8; ++i)
            {
                I8.Add(new ConnectResult { Coefficient = "K1", Index = "I9" });
            }

            Dictionary<string, List<ConnectResult>> connect = new Dictionary<string, List<ConnectResult>>
            {
                { "I1", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I2" },
                        new ConnectResult { Coefficient = "K2", Index = "I8" }
                    }
                },
                { "I2", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I3" },
                        new ConnectResult { Coefficient = "K2", Index = "I4" },
                        new ConnectResult { Coefficient = "K3", Index = "I5" },
                        new ConnectResult { Coefficient = "K4", Index = "I6" },
                        new ConnectResult { Coefficient = "K5", Index = "I7" }
                    }
                },
                { "I8", I8
                },
                { "I9", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I10" },
                        new ConnectResult { Coefficient = "K2", Index = "I15" }   
                    }
                },
                { "I10", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I11" },
                        new ConnectResult { Coefficient = "K2", Index = "I12" },
                        new ConnectResult { Coefficient = "K3", Index = "I13" },
                        new ConnectResult { Coefficient = "K4", Index = "I14" }
                    }
                },
                { "I15", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I16" },
                        new ConnectResult { Coefficient = "K2", Index = "I17" }
                    }
                }
            };

            var profileResult = GetProfileResult(profiles, connect, "I1");

            return Ok(profileResult);
        }

        private void CountI8(ProfileListView listView, ref int count)
        {
            if (listView.Requirements == null || listView.Requirements.Count == 0)
                return;
            if (!string.IsNullOrEmpty(listView.Profile))
            {
                ++count;
            }
            foreach (var requirement in listView.Requirements)
            {
                CountI8(requirement, ref count);
            }
        }

        private ProfileResult GetProfileResult(ProfileListView profileListView, Dictionary<string, List<ConnectResult>> connect, string index)
        {
            ProfileResult result = null;
            result = new ProfileResult();
            result.Name = index;
            result.ProfileResults = new List<ProfileResult>();

            var profiles = JsonSerializer.Deserialize<List<Models.Profile.Profile>>(profileListView.Profile);

            if (!connect.ContainsKey(index))
            {

                var profile = profiles.FirstOrDefault(p => p.NameIndex == index);
                foreach (var coeff in profile.Coefficients)
                {
                    var test = GetProfileResultMetric(profiles, coeff.Name, index, coeff.Metric.Name);
                    result.ProfileResults.Add(test);
                }
            }
            else
            {
                foreach (var test in connect[index])
                {
                    var profile = GetProfileResult(profileListView, connect, test.Index);
                    profile.Coeff = profiles.FirstOrDefault(p => p.NameIndex == index).Coefficients.FirstOrDefault(c => c.Name == test.Coefficient).Value;
                    result.ProfileResults.Add(profile);
                }
            }
            
            
            return result;
        }

        private ProfileResult GetProfileResultMetric(List<Models.Profile.Profile> profiles, string k, string index, string metric)
        {
            ProfileResult result = new ProfileResult();
            result.Name = metric;
            result.Coeff = profiles.FirstOrDefault(p => p.NameIndex == index).Coefficients.FirstOrDefault(c => c.Name == k).Value;
            result.Value = profiles.FirstOrDefault(p => p.NameIndex == index).Coefficients.FirstOrDefault(c => c.Metric.Name == metric).Metric.Value;
            result.ProfileResults = new List<ProfileResult>();

            return result;
        }
    }
}