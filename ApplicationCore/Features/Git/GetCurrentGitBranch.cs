using Infrastructure;

namespace ApplicationCore.Features.Git;

public class GetCurrentGitBranchQuery
{
    public string DirectoryPath { get; set; } = "";
}

public interface IGetCurrentGitBranchService
{
    string Handle(GetCurrentGitBranchQuery query);
}

public class GetCurrentGitBranchService(IGitRepository gitService) : IGetCurrentGitBranchService
{
    private readonly IGitRepository gitService = gitService;

    public string Handle(GetCurrentGitBranchQuery query)
    {
        return gitService.GetCurrentBranch(query.DirectoryPath);
    }
}
