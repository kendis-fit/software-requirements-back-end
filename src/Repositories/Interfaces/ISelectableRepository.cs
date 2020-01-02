namespace SoftwareRequirements.Repositories.Interfaces
{
    public interface ISelectableRepository<T>
    {
        T GetAll(int offset, int size);
    }
}