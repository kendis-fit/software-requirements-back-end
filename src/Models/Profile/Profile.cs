using System.Collections.Generic;

namespace SoftwareRequirements.Models.Profile
{
    class Profile
    {
        public string NameIndex { get; set; }

        public string Name { get; set; }

        public List<Coefficient> Coefficients { get; set; }
    }
}