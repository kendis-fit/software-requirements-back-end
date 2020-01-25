using System.Linq;
using System.Collections.Generic;

using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Algorithm
{
    public class CalculateProfile : ICalculateProfile
    {
        private ProfileResult result;

        public CalculateProfile(ProfileResult result)
        {
            this.result = result;
        }

        public float Calculate()
        {
            return Calculate(this.result);
        }

        private float Calculate(ProfileResult profileResult)
        {
            if(profileResult.Value != null) 
            {
                return profileResult.Value.Value * profileResult.Coeff.Value;
            } 
            else
            {
                var results = new List<float>();
                foreach(ProfileResult r in profileResult.ProfileResults)
                {
                    float result = Calculate(r) * (r.Value.HasValue ? 1 : r.Coeff.Value);
                    results.Add(result);
                }
                return results.Sum();
            }
        }
    }
}