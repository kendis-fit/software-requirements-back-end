namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ICreatableRepository<P>
    {
         void Create(P item);
    }
}