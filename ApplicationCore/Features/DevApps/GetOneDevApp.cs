﻿using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public class GetOneDevAppQuery
    {
        public int Id { get; init; }
    }

    public interface IGetOneDevAppService
    {
        Task<DevAppViewModel> HandleAsync(GetOneDevAppQuery query);
    }

    public class GetOneDevAppService(IDevAppRepository devAppRepository) : IGetOneDevAppService
    {
        public async Task<DevAppViewModel> HandleAsync(GetOneDevAppQuery query)
        {
            var devApp = await devAppRepository.GetById(query.Id);

            return new() { Id = devApp.Id, Path = devApp.Path };
        }
    }
}
