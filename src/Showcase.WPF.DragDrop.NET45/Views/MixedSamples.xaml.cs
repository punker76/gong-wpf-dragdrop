using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
#if !NET35
using System.Threading.Tasks;
#endif
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
            InitializeComponent();
            this.Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var appArgs = Environment.GetCommandLineArgs();
            if (appArgs.Length > 1 && appArgs[1] == "anotherOne")
            {
                MixedTabControl.SelectedItem = MixedTabControl.Items.OfType<TabItem>().FirstOrDefault(t => (string)t.Header == "Outside");
            }
        }

        private void ButtonOpenAnotherAppOnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Assembly.GetEntryAssembly().Location, "anotherOne");
        }
    }
}