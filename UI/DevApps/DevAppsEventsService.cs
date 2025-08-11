namespace UI.DevApps
{
    public interface IDevAppsEventsService
    {
        event EventHandler? OnDevAppsChanged;
        void DevAppsChanged();
    }

    public class DevAppsEventsService : IDevAppsEventsService
    {
        public event EventHandler? OnDevAppsChanged;

        public void DevAppsChanged()
        {
            OnDevAppsChanged!.Invoke(this, EventArgs.Empty);
        }
    }
}
