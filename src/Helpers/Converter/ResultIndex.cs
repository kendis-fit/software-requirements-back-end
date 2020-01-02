using System.Collections.Generic;
using SoftwareRequirements.Repositories;
using SoftwareRequirements.Helpers.Converter.Structs;
using SoftwareRequirements.Helpers.Converter.Connections;
using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter
{
    public class ResultIndex : ResultConverter
    {
        private readonly Dictionary<string, List<ConnectResult>> connector;
        private readonly string index;

        public ResultIndex(ProfileRepository repository, IConnectorProfile connector, string index) : base(repository) 
        {
            this.connector = connector.MakeConnect();
            this.index = index;
        }

        public override ProfileResult Create() => Create(this.index);

        private ProfileResult Create(string index)
        {
            ProfileResult result = new ProfileResult();
            result.Name = index;
            result.ProfileResults = new List<ProfileResult>();

            if (index == "I8")
            {
                result.Coeff = repository.GetCoefficientValue("I1", "K2");
            }
            else if (!connector.ContainsKey(index))
            {
                var profile = repository.FindById(index);
                foreach (var coeff in profile.Coefficients)
                {
                    var resultMetric = new ResultMetric(repository, new SearchMetric(coeff.Name, index, coeff.Metric.Name));
                    var profileResult = resultMetric.Create();
                    result.ProfileResults.Add(profileResult);
                }
            }
            else
            {
                foreach (var test in connector[index])
                {
                    ProfileResult profile = new ProfileResult();
                    profile = Create(test.Index);
                    profile.Coeff = repository.GetCoefficientValue(index, test.Coefficient);
                    result.ProfileResults.Add(profile);   
                }
            }
            return result;
        }
    }
}