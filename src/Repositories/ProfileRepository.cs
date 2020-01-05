using System;
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
            FindById(nameIndex)?.Coefficients.FirstOrDefault(coeff => coeff?.Metric.Name == nameMetric)?.Metric?.Value;

        public List<Profile> AddCoeffToI8()
        {
            var index = FindById("I8");
            if (index != null)
            {
                var coeff = index.Coefficients.LastOrDefault();
                string kIndex = null;
                if (coeff != null)
                {
                    int firstIndexK = coeff.Name.IndexOf("K") + 1;
                    int value = int.Parse(coeff.Name.Substring(firstIndexK)) + 1;

                    kIndex = "K" + value;
                }
                else
                {
                    kIndex = "K1";
                }
                index.Coefficients.Add(new Coefficient
                {
                    Name = kIndex,
                    Value = null
                });
                return profile;
            }
            throw new Exception("Index I8 not found");
        }

        public List<Profile> RemoveLastCoeffsI8(int count)
        {
            var index = FindById("I8");
            if (index != null)
            {
                int lengthCoeffs = index.Coefficients.Count;
                int lengthRemoved = count;

                index.Coefficients = index.Coefficients.Take(lengthCoeffs - lengthRemoved).ToList();
                return profile;
            }
            throw new Exception("Index I8 not found");
        }
    }
}