using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
#if NET35
using GongSolutions.Wpf.DragDrop.Utilities;
#endif

namespace GongSolutions.Wpf.DragDrop
{
  public class DropTargetInsertionAdorner : DropTargetAdorner
  {
    public DropTargetInsertionAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var itemsControl = this.DropInfo.VisualTarget as ItemsControl;

      if (itemsControl != null) {
        // Get the position of the item at the insertion index. If the insertion point is
        // to be after the last item, then get the position of the last item and add an 
        // offset later to draw it at the end of the list.
        ItemsControl itemParent;

        if (this.DropInfo.VisualTargetItem != null) {
          itemParent = ItemsControl.ItemsControlFromItemContainer(this.DropInfo.VisualTargetItem);
        } else {
          itemParent = itemsControl;
        }

        var index = Math.Min(this.DropInfo.InsertIndex, itemParent.Items.Count - 1);

        var lastItemInGroup = false;
        var targetGroup = this.DropInfo.TargetGroup;
        if (targetGroup != null && targetGroup.IsBottomLevel && this.DropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem)) {
          var indexOf = targetGroup.Items.IndexOf(this.DropInfo.TargetItem);
          lastItemInGroup = indexOf == targetGroup.ItemCount - 1;
          if (lastItemInGroup && this.DropInfo.InsertIndex != itemParent.Items.Count) {
            index--;
          }
        }

        var itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

        if (itemContainer != null) {
          var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), this.AdornedElement), itemContainer.RenderSize);
          Point point1, point2;
          double rotation = 0;

          var viewportWidth = DropInfo.TargetScrollViewer?.ViewportWidth ?? double.MaxValue;
          var viewportHeight = DropInfo.TargetScrollViewer?.ViewportHeight ?? double.MaxValue;

          if (this.DropInfo.VisualTargetOrientation == Orientation.Vertical) {
            if (this.DropInfo.InsertIndex == itemParent.Items.Count || lastItemInGroup) {
              itemRect.Y += itemContainer.RenderSize.Height;
            }

            var itemRectRight = Math.Min(itemRect.Right, viewportWidth);
            var itemRectLeft = itemRect.X < 0 ? 0 : itemRect.X;
            point1 = new Point(itemRectLeft, itemRect.Y);
            point2 = new Point(itemRectRight, itemRect.Y);
          } else {
            var itemRectX = itemRect.X;

            if (this.DropInfo.VisualTargetFlowDirection == FlowDirection.LeftToRight && this.DropInfo.InsertIndex == itemParent.Items.Count) {
              itemRectX += itemContainer.RenderSize.Width;
            } else if (this.DropInfo.VisualTargetFlowDirection == FlowDirection.RightToLeft && this.DropInfo.InsertIndex != itemParent.Items.Count) {
              itemRectX += itemContainer.RenderSize.Width;
            }

            point1 = new Point(itemRectX, itemRect.Y);
            point2 = new Point(itemRectX, itemRect.Bottom);
            rotation = 90;
          }

          drawingContext.DrawLine(m_Pen, point1, point2);
          this.DrawTriangle(drawingContext, point1, rotation);
          this.DrawTriangle(drawingContext, point2, 180 + rotation);
        }
      }
    }

    private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
    {
      drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
      drawingContext.PushTransform(new RotateTransform(rotation));

      drawingContext.DrawGeometry(m_Pen.Brush, null, m_Triangle);

      drawingContext.Pop();
      drawingContext.Pop();
    }

    static DropTargetInsertionAdorner()
    {
      // Create the pen and triangle in a static constructor and freeze them to improve performance.
      const int triangleSize = 5;

      m_Pen = new Pen(Brushes.Gray, 2);
      m_Pen.Freeze();

      var firstLine = new LineSegment(new Point(0, -triangleSize), false);
      firstLine.Freeze();
      var secondLine = new LineSegment(new Point(0, triangleSize), false);
      secondLine.Freeze();

      var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
      figure.Segments.Add(firstLine);
      figure.Segments.Add(secondLine);
      figure.Freeze();

      m_Triangle = new PathGeometry();
      m_Triangle.Figures.Add(figure);
      m_Triangle.Freeze();
    }

    private static readonly Pen m_Pen;
    private static readonly PathGeometry m_Triangle;
  }
}