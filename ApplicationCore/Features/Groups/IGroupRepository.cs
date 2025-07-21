namespace ApplicationCore.Features.Groups
{
    public interface IGroupRepository
    {
        Task<Group> GetOne(long id);
        Task<IEnumerable<Group>> GetAll();
        Task<bool> Add(Group param);
        Task<bool> Edit(Group param);
        Task<bool> Delete(long id);
    }
}
