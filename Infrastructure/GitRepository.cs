using LibGit2Sharp;

namespace Infrastructure;

public interface IGitRepository
{
    string GetCurrentBranch(string projectPath);
}

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
        catch (RepositoryNotFoundException ex)
        {
            return ex.Message;
        }
    }
}
