using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#else
using System.Windows.Controls.Primitives;
#endif

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  public static class HitTestUtilities
  {
    public static bool HitTest4Type<T>(object sender, Point elementPosition) where T : UIElement
    {
      var uiElement = GetHitTestElement4Type<T>(sender, elementPosition);
      return uiElement != null && uiElement.Visibility == Visibility.Visible;
    }

    private static T GetHitTestElement4Type<T>(object sender, Point elementPosition) where T : UIElement
    {
      var hit = VisualTreeHelper.HitTest((Visual)sender, elementPosition);
      if (hit == null) {
        return null;
      } else {
        var uiElement = hit.VisualHit.GetVisualAncestor<T>();
        return uiElement;
      }
    }

    public static bool HitTest4GridViewColumnHeader(object sender, Point elementPosition)
    {
      if (sender is ListView) {
        // no drag&drop for column header
        var columnHeader = GetHitTestElement4Type<GridViewColumnHeader>(sender, elementPosition);
        if (columnHeader != null && (columnHeader.Role == GridViewColumnHeaderRole.Floating || columnHeader.Visibility == Visibility.Visible)) {
          return true;
        }
      }
      return false;
    }

    public static bool HitTest4DataGridTypes(object sender, Point elementPosition)
    {
      if (sender is DataGrid) {
        // no drag&drop for column header
        var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
        if (columnHeader != null && columnHeader.Visibility == Visibility.Visible) {
          return true;
        }
        // no drag&drop for row header
        var rowHeader = GetHitTestElement4Type<DataGridRowHeader>(sender, elementPosition);
        if (rowHeader != null && rowHeader.Visibility == Visibility.Visible) {
          return true;
        }
        // drag&drop only for data grid row
        var dataRow = GetHitTestElement4Type<DataGridRow>(sender, elementPosition);
        return dataRow == null;
      }
      return false;
    }

    public static bool HitTest4DataGridTypesOnDragOver(object sender, Point elementPosition)
    {
      if (sender is DataGrid) {
        // no drag&drop on column header
        var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
        if (columnHeader != null && columnHeader.Visibility == Visibility.Visible) {
          return true;
        }
      }
      return false;
    }
  }
}