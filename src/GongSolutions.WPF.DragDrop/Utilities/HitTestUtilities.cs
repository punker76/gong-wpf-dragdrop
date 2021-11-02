using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class HitTestUtilities
    {
        public static bool HitTest4Type<T>(object sender, Point elementPosition)
            where T : UIElement
        {
            var uiElement = GetHitTestElement4Type<T>(sender, elementPosition);
            return uiElement is { Visibility: Visibility.Visible };
        }

        private static T GetHitTestElement4Type<T>(object sender, Point elementPosition)
            where T : UIElement
        {
            if (sender is not Visual visual)
            {
                return null;
            }

            var hit = VisualTreeHelper.HitTest(visual, elementPosition);

            return hit?.VisualHit.GetVisualAncestor<T>();
        }

        public static bool HitTest4GridViewColumnHeader(object sender, Point elementPosition)
        {
            if (sender is ListView)
            {
                // no drag&drop for column header
                var columnHeader = GetHitTestElement4Type<GridViewColumnHeader>(sender, elementPosition);
                if (columnHeader != null && (columnHeader.Role == GridViewColumnHeaderRole.Floating || columnHeader.Visibility == Visibility.Visible))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HitTest4DataGridTypes(object sender, Point elementPosition)
        {
            if (sender is not DataGrid)
            {
                return false;
            }

            // no drag&drop for column header
            var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
            if (columnHeader is { Visibility: Visibility.Visible })
            {
                return true;
            }

            // no drag&drop for row header
            var rowHeader = GetHitTestElement4Type<DataGridRowHeader>(sender, elementPosition);
            if (rowHeader is { Visibility: Visibility.Visible })
            {
                // no drag&drop for row header gripper
                var thumb = GetHitTestElement4Type<Thumb>(sender, elementPosition);
                if (thumb != null)
                {
                    return true;
                }
            }

            // drag&drop only for data grid row
            var dataRow = GetHitTestElement4Type<DataGridRow>(sender, elementPosition);
            return dataRow == null || Equals(dataRow.DataContext, CollectionView.NewItemPlaceholder);
        }

        public static bool HitTest4DataGridTypesOnDragOver(object sender, Point elementPosition)
        {
            if (sender is not DataGrid)
            {
                return false;
            }

            // no drag&drop on column header
            var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
            if (columnHeader is { Visibility: Visibility.Visible })
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// thx to @osicka from issue #84
        /// 
        /// e.g. original source is part of a popup (e.g. ComboBox drop down), the hit test needs to be done on the original source.
        /// Because the popup is not attached to the visual tree of the sender.
        /// This function test this by looping back from the original source to the sender and if it didn't end up at the sender stopped the drag.
        /// </summary>
        public static bool IsNotPartOfSender(object sender, MouseButtonEventArgs e)
        {
            return IsNotPartOfSender(sender, e.OriginalSource, e.GetPosition((IInputElement)e.OriginalSource));
        }

        public static bool IsNotPartOfSender(object sender, TouchEventArgs e)
        {
            return IsNotPartOfSender(sender, e.OriginalSource, e.GetTouchPoint((IInputElement)e.OriginalSource).Position);
        }

        private static bool IsNotPartOfSender(object sender, object originalSource, Point position)
        {
            if (originalSource is not Visual visual)
            {
                return false;
            }

            var hit = VisualTreeHelper.HitTest(visual, position);
            if (hit is null)
            {
                return false;
            }

            if (visual == sender)
            {
                return false;
            }

            var parent = VisualTreeHelper.GetParent(visual.FindVisualTreeRoot());
            //var item = VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject);

            while (parent != null && parent != sender)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent != sender;
        }
    }
}