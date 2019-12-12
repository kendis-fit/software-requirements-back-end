using System.Text.Json;
using System.Collections.Generic;

namespace SoftwareRequirements.Models.DTO
{
    public class ProfileListView
    {
        public string Name { get; set; }

        public string Profile { get; set; }

        public List<ProfileListView> Requirements { get; set; }
    }
}