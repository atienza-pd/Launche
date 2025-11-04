namespace ApplicationCore.Features.Groups;

public class GetAllGroupViewModel
{
    public IEnumerable<Group> Groups { get; init; } = [];
}

public interface IGroupService
{
    Task<GetAllGroupViewModel> GetAll();
}

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
    private readonly IGroupRepository groupRepository = groupRepository;

    public async Task<GetAllGroupViewModel> GetAll()
    {
        var groups = await groupRepository.GetAll();

        return new()
        {
            Groups = groups.Select(group => new Group()
            {
                Id = group.Id,
                Name = group.Name,
            }),
        };
    }
}
