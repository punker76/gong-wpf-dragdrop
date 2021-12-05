using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Showcase.WPF.DragDrop.Views
{
    /// <summary>
    /// Interaction logic for MixedSamples.xaml
    /// </summary>
    public partial class MixedSamples : UserControl
    {
        public MixedSamples()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var appArgs = Environment.GetCommandLineArgs();
            if (appArgs.Length > 1 && appArgs[1] == "anotherOne")
            {
                this.MixedTabControl.SelectedItem = this.MixedTabControl.Items.OfType<TabItem>().FirstOrDefault(t => (string)t.Header == "Outside");
            }
        }

        private void ButtonOpenAnotherAppOnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Assembly.GetEntryAssembly().Location, "anotherOne");
        }
    }
}