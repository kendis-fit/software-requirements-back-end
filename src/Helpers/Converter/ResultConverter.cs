using SoftwareRequirements.Repositories;
using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter
{
    abstract public class ResultConverter
    {
        protected ProfileRepository repository;

        public ResultConverter(ProfileRepository repository) => this.repository = repository;
        
        abstract public ProfileResult Create();
    }
}