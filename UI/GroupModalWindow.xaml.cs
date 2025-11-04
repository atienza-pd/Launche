using ApplicationCore.Features.Projects;
using System.Windows;


namespace UI.Windows.Group;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    public ProjectViewModel? ProjectPath { get; set; }
    public event EventHandler? OnSave;


    public GroupModalWindow()
    {
        InitializeComponent();
        //DataContext = dataContext;
    }



    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (ProjectPath == null)
        {
            return;
        }


        this.ListBoxGroup.SelectedItem = null;


    }

    private void ListBoxGroup_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
    )
    {

    }
}
