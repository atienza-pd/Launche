using Infrastructure.Repositories;

namespace ApplicationCore.Features.Groups;

public class GetAllGroupViewModel
{
    public IEnumerable<GroupViewModel> Groups { get; init; } = [];
}

public class GetAllGroupQuery { }

public interface IGetAllGroupService
{
    Task<GetAllGroupViewModel> HandleAsync(GetAllGroupQuery query);
}

public class GetAllGroupService(IGroupRepository groupRepository) : IGetAllGroupService
{
    private readonly IGroupRepository groupRepository = groupRepository;

    public async Task<GetAllGroupViewModel> HandleAsync(GetAllGroupQuery query)
    {
        var groups = await groupRepository.GetAll();

        return new()
        {
            Groups = groups.Select(group => new GroupViewModel()
            {
                Id = group.Id,
                Name = group.Name,
            }),
        };
    }
}
