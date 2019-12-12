using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftwareRequirements.Models
{
    class Coefficient
    {
        [Required]
        public string Name { get; set; }

        public int? Value { get; set; }

        public string? NameIndex { get; set; }

        public List<Primitive>? Primitives { get; set; }
    }
}