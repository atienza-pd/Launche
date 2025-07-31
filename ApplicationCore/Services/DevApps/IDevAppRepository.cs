namespace ApplicationCore.Features.DevApps;

public interface IDevAppRepository
{
    Task<bool> Add(DevApp param);
    Task<bool> Edit(DevApp param);
    Task<bool> Delete(long id);
    Task<DevApp> GetById(int id);
    Task<IEnumerable<DevApp>> GetAll();
}
