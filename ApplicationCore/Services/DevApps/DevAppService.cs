namespace ApplicationCore.Features.DevApps;

public interface IDevAppService
{
    Task<bool> Add(DevApp param);
    Task<bool> Edit(DevApp param);
    Task<bool> Delete(long id);
    Task<DevApp> GetById(int id);
    Task<IEnumerable<DevAppViewModel>> GetAll();
}

public class DevAppService(IDevAppRepository devAppRepository) : IDevAppService
{
    private readonly IDevAppRepository devAppRepository = devAppRepository;

    public async Task<bool> Add(DevApp param)
    {
        ArgumentNullException.ThrowIfNull(param);

        if (string.IsNullOrEmpty(param.Name))
        {
            throw new ApplicationException($"{nameof(param.Name)} is required!");
        }

        if (string.IsNullOrEmpty(param.Path))
        {
            throw new ApplicationException($"{nameof(param.Path)} is required!");
        }

        if (!param.Path.Contains(".exe", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ApplicationException($"{nameof(param.Path)} must be executable file!");
        }

        return await devAppRepository.Add(new() { Path = param.Path, Name = param.Name });
    }

    public async Task<bool> Delete(long id)
    {
        return await devAppRepository.Delete(id);
    }

    public async Task<bool> Edit(DevApp param)
    {
        ArgumentNullException.ThrowIfNull(param);

        if (string.IsNullOrEmpty(param.Name))
        {
            throw new ApplicationException($"{nameof(param.Name)} is required!");
        }

        if (string.IsNullOrEmpty(param.Path))
        {
            throw new ApplicationException($"{nameof(param.Path)} is required!");
        }

        if (!param.Path.Contains(".exe", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ApplicationException($"{nameof(param.Path)} must be executable file!");
        }

        return await devAppRepository.Edit(new() { Path = param.Path, Id = param.Id, Name = param.Name });
    }

    public async Task<IEnumerable<DevAppViewModel>> GetAll()
    {
        var devApps = await devAppRepository.GetAll();

        return devApps.Select(devApp => new DevAppViewModel
        {
            Id = devApp.Id,
            Path = devApp.Path,
            Name = devApp.Name
        });
    }

    public async Task<DevApp> GetById(int id)
    {
        var devApp = await devAppRepository.GetById(id);

        return new() { Id = devApp.Id, Path = devApp.Path, Name = devApp.Name };
    }
}
