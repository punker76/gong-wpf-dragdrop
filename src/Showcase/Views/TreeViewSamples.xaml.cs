using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace Showcase.WPF.DragDrop.Views
{
    /// <summary>
    /// Interaction logic for TreeViewSamples.xaml
    /// </summary>
    public partial class TreeViewSamples : UserControl
    {
        public TreeViewSamples()
        {
            InitializeComponent();
        }

        private void LeftBoundTreeView_RequestBringIntoView(object sender, System.Windows.RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void LeftBoundTreeView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Prevent automatic horizontal and vertical scrolling by subscribing to ScrollContentPresenter's RequestBringIntoView event and flag it as handled.
            var p = (sender as TreeView).GetVisualDescendent<ScrollContentPresenter>();
            if (p != null)
            {
                p.RequestBringIntoView -= this.LeftBoundTreeView_RequestBringIntoView;
                p.RequestBringIntoView += this.LeftBoundTreeView_RequestBringIntoView;
            }
        }
    }
}