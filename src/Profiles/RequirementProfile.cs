using AutoMapper;

using SoftwareRequirements.Models.Db;
using SoftwareRequirements.Models.DTO;

namespace SoftwareRequirements.Profiles
{
    public class RequirementProfile : Profile
    {
        public RequirementProfile()
        {
            CreateMap<Requirement, RequirementView>();
            CreateMap<Requirement, RequirementListView>();
            CreateMap<Requirement, ProfileView>();
            CreateMap<Requirement, ProfileListView>();
        }
    }
}