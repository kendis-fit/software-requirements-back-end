using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftwareRequirements.Models.Profile
{
    class Coefficient
    {
        [Required]
        public string Name { get; set; }

        public float? Value { get; set; }

        public Metric Metric { get; set; }
    }
}