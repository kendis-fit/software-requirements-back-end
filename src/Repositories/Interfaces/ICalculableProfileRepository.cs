using System.Threading.Tasks;
using System.Collections.Generic;

using SoftwareRequirements.Models.DTO;

namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ICalculableProfileRepository
    {
        Task<float> Calculate(int id, string indexId);

        Task<List<ProfileRadarResult>> ConvertToDiagram(int id, string indexId);
    }
}