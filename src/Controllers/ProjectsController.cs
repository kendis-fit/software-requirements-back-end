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

            var profiles = JsonSerializer.Deserialize<List<Models.Profile.Profile>>(profileListView.Profile);

            Dictionary<string, List<ConnectResult>> baseConnect = new Dictionary<string, List<ConnectResult>>
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
                }
            };

            Dictionary<string, List<ConnectResult>> connect = new Dictionary<string, List<ConnectResult>>
            {
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

            var projectProfileResult = GetProfileResult(profiles, baseConnect, "I1");

            var requirementProfileResult = projectProfileResult.ProfileResults.FirstOrDefault(p => p.Name == "I8");

            var i8 = profiles.FirstOrDefault(p => p.NameIndex == "I8");

            foreach (var coeff in i8.Coefficients.Select((value, i) => new { value, i}))
            {
                int count = 0;
                var requirementProfile = new List<Models.Profile.Profile>();
                GetRequiremetProfile(profileListView, coeff.i + 1, ref count, ref requirementProfile);
                var profileResult = GetProfileResult(requirementProfile, connect, "I9");
                profileResult.Coeff = coeff.value.Value;
                requirementProfileResult.ProfileResults.Add(profileResult);
            }

            float result = calculateStuff(projectProfileResult);

            return Ok(result);
        }

        private float calculateStuff(ProfileResult res)
        {
            if(res.Value != null) return res.Value.Value * res.Coeff.Value;
            else {
                var results = new List<float>();
                foreach(ProfileResult r in res.ProfileResults) {
                    float result = calculateStuff(r);
                    results.Add(result);
                }

                return results.Sum();
            }
        }

        private void GetRequiremetProfile(ProfileListView listView, int index, ref int count, ref List<Models.Profile.Profile> profile)
        {
            if (index == count)
            {
                profile = JsonSerializer.Deserialize<List<Models.Profile.Profile>>(listView.Profile);
            }

            foreach (var requirement in listView.Requirements)
            {
                if (!string.IsNullOrEmpty(requirement.Profile)) ++count;
                GetRequiremetProfile(requirement, index, ref count, ref profile);
            }
        } 

        private ProfileResult GetProfileResult(List<Models.Profile.Profile> profiles, Dictionary<string, List<ConnectResult>> connect, string index)
        {
            ProfileResult result = null;
            result = new ProfileResult();
            result.Name = index;
            result.ProfileResults = new List<ProfileResult>();

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
                    ProfileResult profile = null;
                    if (test.Index != "I8")
                    {
                        profile = GetProfileResult(profiles, connect, test.Index);
                    }
                    else
                    {
                        profile = new ProfileResult();
                        profile.Name = test.Index;
                        profile.ProfileResults = new List<ProfileResult>();
                    }
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