using SoftwareRequirements.Models.Db;

namespace SoftwareRequirements.Models.DTO
{
    public class RequirementListView
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public RequirementWrite Write { get; set; }
    }
}