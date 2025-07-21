namespace ApplicationCore.Features.Git;


public interface IGitService
{
    string GetCurrentBranch(string projectPath);
}

public class GitService(IGitRepository gitService) : IGitService
{
    private readonly IGitRepository gitService = gitService;

    public string GetCurrentBranch(string projectPath)
    {
        try
        {
            return gitService.GetCurrentBranch(projectPath);
        }
        catch (Exception ex)
        {
            throw new GitServiceException("No Git Branch Found!", ex);
        }
    }
}
