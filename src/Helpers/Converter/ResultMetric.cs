using System.Collections.Generic;

using SoftwareRequirements.Repositories;
using SoftwareRequirements.Helpers.Converter.Structs;
using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter
{
    public class ResultMetric : ResultConverter
    {
        private readonly SearchMetric searchMetric;

        public ResultMetric(ProfileRepository repository, SearchMetric searchMetric) : base(repository) 
        {
            this.searchMetric = searchMetric;
        }

        public override ProfileResult Create()
        {
            ProfileResult result = new ProfileResult();
            result.Name = searchMetric.Metric;
            result.Coeff = repository.GetCoefficientValue(searchMetric.Index, searchMetric.Coefficient);
            result.Value = repository.GetMetricValue(searchMetric.Index, searchMetric.Metric);
            result.ProfileResults = new List<ProfileResult>();

            return result;
        }
    }
}