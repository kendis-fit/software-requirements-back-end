using System.Collections.Generic;
using System;
using System.IO;
using AutoMapper;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileReader = System.IO.File;
using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Db;
using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Models.Profile;
using SoftwareRequirements.Helpers.Algorithm;
using SoftwareRequirements.Helpers.Converter;

using SoftwareRequirements.Exceptions;
using SoftwareRequirements.Repositories;


namespace SoftwareRequirements.Controllers
{
    [Route("[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly RequirementRepository repository;

        public RequirementsController(RequirementRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost] // YOU DON'T FORGET CHANGE A METHOD IN FRONT-END, YOU SHOULD SEND PARENT-ID INSIDE BODY OF REQUEST
        public async Task<IActionResult> CreateRequirements([FromBody]RequirementCreate requirement)
        {
            try
            {
                var newRequirement = await repository.Create(requirement);
                return Created($"/profiles/{newRequirement.Id}", newRequirement.Id);
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirementById(int id)
        {
            try
            {
                await repository.Remove(id);
                return Ok();
            }
            catch (RestException ex)
            {
                return ex.SendStatusCode();
            }
        }

        // [HttpPatch("{id}")]
        // public async Task<IActionResult> UpdateNameRequirement(int id, [FromBody]RequirementCreate requirementChange)
        // {
        //     using var transaction = await db.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var requirement = await db.Requirements.FirstOrDefaultAsync(r => r.Id == id && r.Parent != null);
        //         if (requirement == null)
        //             return NotFound();
        //         requirement.Name = requirementChange.Name;

        //         db.Requirements.Update(requirement);
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
        public async Task<IActionResult> GetResult(int id, string indexId)
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