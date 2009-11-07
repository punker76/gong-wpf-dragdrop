using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public class DragInfo
    {
        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            DragStartPosition = e.GetPosition(null);
            Effects = DragDropEffects.None;
            MouseButton = e.ChangedButton;
            VisualSource = sender as UIElement;

            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                UIElement item = itemsControl.GetItemContainer((UIElement)e.OriginalSource);

                if (item != null)
                {
                    ItemsControl itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    SourceCollection = itemParent.ItemsSource ?? itemParent.Items;
                    SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    SourceItems = itemsControl.GetSelectedItems();

                    // Some controls (I'm looking at you TreeView!) haven't updated their
                    // SelectedItem by this point. Check to see if there 1 or less item in 
                    // the SourceItems collection, and if so, override the SelectedItems
                    // with the clicked item.
                    if (SourceItems.Cast<object>().Count() <= 1)
                    {
                        SourceItems = Enumerable.Repeat(SourceItem, 1);
                    }

                    VisualSourceItem = item;
                }
                else
                {
                    SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }

            if (SourceItems == null)
            {
                SourceItems = Enumerable.Empty<object>();
            }
        }

        public object Data { get; set; }
        public Point DragStartPosition { get; private set; }
        public DragDropEffects Effects { get; set; }
        public MouseButton MouseButton { get; private set; }
        public IEnumerable SourceCollection { get; private set; }
        public object SourceItem { get; private set; }
        public IEnumerable SourceItems { get; private set; }
        public UIElement VisualSource { get; private set; }
        public UIElement VisualSourceItem { get; private set; }
    }
}
