using ApplicationCore.Features.Git;
using LibGit2Sharp;

namespace Infrastructure;

public class GitRepository : IGitRepository
{

    public string GetCurrentBranch(string projectPath)
    {
        try
        {
            var repo = new Repository(projectPath);
            var branch = repo.Branches.FirstOrDefault(branch =>
                branch.IsCurrentRepositoryHead
            );

            return $"Current Git Branch: {branch?.FriendlyName}" ?? "";
        }
        catch (RepositoryNotFoundException)
        {
            throw new ApplicationException("No Respository found for this Branch!");
        }
    }
}
