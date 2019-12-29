using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftwareRequirements.Models.Profile
{
    public class Metric
    {
        [Required]
        public string Name { get; set; }

        public string NameMetric { get; set; }

        public float? Value { get; set; }

        public List<Primitive> Primitives { get; set; }
    }
}