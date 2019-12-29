using System.Linq;
using System.Collections.Generic;

using SoftwareRequirements.Models.Profile;
using SoftwareRequirements.Repositories.Interfaces;

namespace SoftwareRequirements.Repositories
{
    public class ProfileRepository : ISearchableRepository<Profile, string>
    {
        private readonly List<Profile> profile;

        public ProfileRepository(List<Profile> profile)
        {
            this.profile = profile;
        }

        public Profile FindById(string nameIndex) => this.profile.FirstOrDefault(index => index.NameIndex == nameIndex);

        public float? GetCoefficientValue(string nameIndex, string nameCoefficient) => 
            FindById(nameIndex)?.Coefficients.FirstOrDefault(coeff => coeff.Name == nameCoefficient)?.Value;

        public float? GetMetricValue(string nameIndex, string nameMetric) =>
            FindById(nameIndex)?.Coefficients.FirstOrDefault(coeff => coeff?.Metric.Name == nameMetric)?.Metric.Value;
    }
}