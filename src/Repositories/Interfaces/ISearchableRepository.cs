namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ISearchableRepository<T, P>
    {
        T FindById(P item);
    }
}