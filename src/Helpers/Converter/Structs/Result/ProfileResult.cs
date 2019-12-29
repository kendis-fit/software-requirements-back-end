using System.Collections.Generic;

namespace SoftwareRequirements.Helpers.Converter.Structs.Result
{
    public struct ProfileResult
    {
        public string Name;

        public float? Coeff;

        public float? Value;

        public List<ProfileResult> ProfileResults; 
    }
}