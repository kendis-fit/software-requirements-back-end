using SoftwareRequirements.Models.Db;

namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ICalculableProfileRepository
    {
        float Calculate(Requirement project, string indexId);    
    }
}