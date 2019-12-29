using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftwareRequirements.Models.Profile
{
    public class Coefficient
    {
        [Required]
        public string Name { get; set; }

        public float? Value { get; set; }

        public Metric Metric { get; set; }
    }
}