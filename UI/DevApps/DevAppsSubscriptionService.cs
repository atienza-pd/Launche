namespace UI.DevApps
{
    public interface IDevAppsSubscriptionService
    {
        event EventHandler? Subscribe;
        void Create();
    }

    public class DevAppsSubscriptionService : IDevAppsSubscriptionService
    {
        public event EventHandler? Subscribe;

        public void Create()
        {
            Subscribe!.Invoke(this, EventArgs.Empty);
        }
    }
}
