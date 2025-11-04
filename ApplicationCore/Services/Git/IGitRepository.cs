namespace ApplicationCore.Features.Git
{
    public interface IGitRepository
    {
        string GetCurrentBranch(string projectPath);
    }
}
