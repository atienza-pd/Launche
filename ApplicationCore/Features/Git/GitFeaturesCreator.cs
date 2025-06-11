using Infrastructure;

namespace ApplicationCore.Features.Git
{
    public interface IGitFeaturesCreator
    {
        IGetCurrentGitBranchService CreateGetCurrentGitBranchService();
    }

    internal class GitFeaturesCreator(IGitRepository gitService) : IGitFeaturesCreator
    {
        private readonly IGitRepository gitService = gitService;

        public IGetCurrentGitBranchService CreateGetCurrentGitBranchService()
        {
            return new GetCurrentGitBranchService(gitService);
        }
    }
}
