namespace MyList.Server.Data.Repositories
{
    public interface IRepository
    {
        public Task<int> SaveChangesAsync();
    }
}
