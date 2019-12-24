using System.Collections.Generic;
namespace SoftwareRequirements.Models
{
    public class ProfileResult
    {
        public string Name { get; set; }

        public float? Coeff { get; set; }

        public float? Value { get; set; }

        public List<ProfileResult> ProfileResults { get; set; } 
    }
}