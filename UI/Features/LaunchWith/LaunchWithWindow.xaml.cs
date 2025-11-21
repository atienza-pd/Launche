using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI.Features.LaunchWith
{
    public interface ILaunchWithWindow
    {
        bool? ShowDialog();
    }

    /// <summary>
    /// Interaction logic for OpenWith.xaml
    /// </summary>
    public partial class LaunchWithWindow : Window, ILaunchWithWindow
    {
        private readonly LaunchWithWindowViewModel openWithWindowViewModel;

        public LaunchWithWindow(LaunchWithWindowViewModel openWithWindowViewModel)
        {
            InitializeComponent();
            DataContext = openWithWindowViewModel;
            this.openWithWindowViewModel = openWithWindowViewModel;
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private async void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            await this.openWithWindowViewModel.Init();
            if (sender is ListView listView && listView.View is GridView gridView)
            {
                // Get the total width of the ListView
                double totalWidth = listView.ActualWidth;

                // Subtract some space for padding, scrollbars, etc.
                double availableWidth = totalWidth - 20; // Adjust as needed

                // Divide the available width between columns
                if (gridView.Columns.Count == 2) // Assuming 2 columns: Name and Actions
                {
                    gridView.Columns[0].Width = availableWidth * 0.85; // 80% for Name column
                    gridView.Columns[1].Width = availableWidth * 0.15; // 10% for Actions column
                }
            }
        }
    }
}
