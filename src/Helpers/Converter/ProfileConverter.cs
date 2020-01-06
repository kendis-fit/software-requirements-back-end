using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using SoftwareRequirements.Repositories;
using SoftwareRequirements.Models.DTO;
using SoftwareRequirements.Models.Profile;
using SoftwareRequirements.Helpers.Converter.Connections;
using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter
{
    public class ProfileConverter
    {
        private readonly ProfileListView profileListView;
        private readonly ProfileRepository repository;
        private readonly IConnectorProfile connector;
        private readonly (int, string) index;

        public ProfileConverter(ProfileListView profileListView, string index)
        {
            var profile = JsonSerializer.Deserialize<List<Profile>>(profileListView.Profile);
            this.repository = new ProfileRepository(profile);
            this.profileListView = profileListView;
            if (Int32.TryParse(index.Replace("I", ""), out this.index.Item1))
            {
                this.index.Item2 = index;
                if (this.index.Item1 >= 1 && this.index.Item1 <= 8)
                {
                    connector = new BaseConnectorProfile();
                }
                else
                {
                    connector = new ConnectorProfile();
                }
            }
        }

        public ProfileResult Convert()
        {
            var resultIndex = new ResultIndex(repository, connector, index.Item2);
            var result = resultIndex.Create();
            if (index.Item1 == 1 || index.Item1 == 8)
            {
                var connectorProfile = new ConnectorProfile();

                ProfileResult requirementProfileResult = new ProfileResult();

                if (index.Item1 == 1)
                {
                    requirementProfileResult = result.ProfileResults.FirstOrDefault(p => p.Name == "I8");
                }
                else
                {
                    requirementProfileResult = result;
                }
        
                var i8 = repository.FindById("I8");

                foreach (var coeff in i8.Coefficients.Select((value, i) => new { value, i}))
                {
                    int count = 0;
                    var requirementProfile = new List<Profile>();
                    GetRequiremetProfile(profileListView, coeff.i + 1, ref count, ref requirementProfile);
                    var newRepository = new ProfileRepository(requirementProfile);
                    var profileResult = new ResultIndex(newRepository, connectorProfile, "I9").Create();
                    profileResult.Coeff = coeff.value.Value;
                    requirementProfileResult.ProfileResults.Add(profileResult);
                }
            }
            return result;
        }

        private void GetRequiremetProfile(ProfileListView listView, int index, ref int count, ref List<Profile> profile)
        {
            if (index == count)
            {
                profile = JsonSerializer.Deserialize<List<Profile>>(listView.Profile);
            }

            foreach (var requirement in listView.Requirements)
            {
                if (!string.IsNullOrEmpty(requirement.Profile)) ++count;
                GetRequiremetProfile(requirement, index, ref count, ref profile);
            }
        } 
    }
}