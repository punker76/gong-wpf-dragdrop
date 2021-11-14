using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Showcase.WPF.DragDrop.ViewModels;

namespace Showcase.WPF.DragDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            this.Loaded += this.MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var appArgs = Environment.GetCommandLineArgs();
            if (appArgs.Length > 1 && appArgs[1] == "anotherOne")
            {
                this.MainTabControl.SelectedItem = this.MainTabControl.Items.OfType<TabItem>().FirstOrDefault(t => (string)t.Header == "Mixed");
            }
        }
    }
}