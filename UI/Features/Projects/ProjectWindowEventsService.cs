namespace UI.Features.Projects;


public interface IProjectWindowEventsService
{
    event EventHandler? OnProjectsChanged;
    void ProjectsChanged();
}

public class ProjectWindowEventsService : IProjectWindowEventsService
{


    public event EventHandler? OnProjectsChanged;

    public void ProjectsChanged()
    {
        OnProjectsChanged!.Invoke(this, EventArgs.Empty);
    }
}
