﻿using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public class GetAllDevAppViewModel
    {
        public IEnumerable<IDEPathViewModel> DevApps { get; init; } = [];
    }

    public interface IGetAllDevAppService
    {
        Task<GetAllDevAppViewModel> HandleAsync();
    }

    public class GetAllDevAppService(IDevAppRepository devAppRepository) : IGetAllDevAppService
    {
        public async Task<GetAllDevAppViewModel> HandleAsync()
        {
            var devApps = await devAppRepository.GetAll();
            return new()
            {
                DevApps = devApps.Select(devApp => new IDEPathViewModel
                {
                    Id = devApp.Id,
                    Path = devApp.Path,
                }),
            };
        }
    }
}
