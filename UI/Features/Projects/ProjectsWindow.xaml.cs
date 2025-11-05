using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Features.Projects;

namespace UI.Features
{
    public interface IProjectsWindow
    {
        bool? ShowDialog();
    }

    /// <summary>
    /// Interaction logic for ProjectsWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window, IProjectsWindow
    {
        private readonly ProjectsWindowViewModel viewModel;

        public ProjectsWindow(ProjectsWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            this.viewModel = viewModel;
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is ListView listView && listView.View is GridView gridView)
            {
                // Get the total width of the ListView
                double totalWidth = listView.ActualWidth;

                // Subtract some space for padding, scrollbars, etc.
                double availableWidth = totalWidth - 10; // Adjust as needed

                // Divide the available width between columns
                if (gridView.Columns.Count == 2) // Assuming 2 columns: Name and Actions
                {
                    gridView.Columns[0].Width = availableWidth * 0.8; // 80% for Name column
                    gridView.Columns[1].Width = availableWidth * 0.2; // 30% for Actions column
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadProjects();
            viewModel.LoadDevApps();
            this.viewModel.SetSelectedProject();
        }

        private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.IsDropDownOpen = true;
            }
        }
    }
}
