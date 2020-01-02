namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ICreatableRepository<T, P>
    {
         T Create(P item);
    }
}