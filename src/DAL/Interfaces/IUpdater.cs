namespace DAL.Interfaces
{
    public interface IUpdater<T>
    {
        void Update(T item);
    }
}
