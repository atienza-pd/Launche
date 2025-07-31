namespace ApplicationCore.Features.Projects
{
    public interface IProjectRepository
    {
        Task<Project> GetLast();
        Task<Project> GetOne(long id);
        Task<IEnumerable<Project>> GetAll();
        Task<bool> Add(Project param);
        Task<bool> Edit(Project param);
        Task<bool> Delete(long id);
        Task<bool> SortUp(int sortId);
        Task<bool> SortDown(int sortId);
    }
}
