using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
  public class DropTargetHighlightAdorner : DropTargetAdorner
  {
    public DropTargetHighlightAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var visualTargetItem = this.DropInfo.VisualTargetItem;
      if (visualTargetItem != null) {
        var rect = Rect.Empty;

        var tvItem = visualTargetItem as TreeViewItem;
        if (tvItem != null && VisualTreeHelper.GetChildrenCount(tvItem) > 0) {
          var grid = VisualTreeHelper.GetChild(tvItem, 0) as Grid;
          if (grid != null) {
            var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
            rect = new Rect(tvItem.TranslatePoint(new Point(), this.AdornedElement), new Size(descendant.Width + 4, grid.RowDefinitions[0].ActualHeight));
          }
        }
        if (rect.IsEmpty) {
          rect = new Rect(visualTargetItem.TranslatePoint(new Point(), this.AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
        }
        drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
      }
    }
  }
}