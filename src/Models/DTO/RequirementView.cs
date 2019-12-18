using System.Collections.Generic;

using SoftwareRequirements.Models.Db;

namespace SoftwareRequirements.Models.DTO
{
    public class RequirementView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<RequirementView> Requirements { get; set; }

        public RequirementWrite Write { get; set; }
    }
}