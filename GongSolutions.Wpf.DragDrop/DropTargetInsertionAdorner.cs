using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;

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
            ItemsControl itemsControl = DropInfo.VisualTarget as ItemsControl;
            
            if (itemsControl != null)
            {
                // Get the position of the item at the insertion index. If the insertion point is
                // to be after the last item, then get the position of the last item and add an 
                // offset later to draw it at the end of the list.
                ItemsControl itemParent;

                if (DropInfo.VisualTargetItem != null)
                {
                    itemParent = ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem);
                }
                else
                {
                    itemParent = itemsControl;
                }

                int index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);
                UIElement itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

                if (itemContainer != null)
                {
                    Rect itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement),
                        itemContainer.RenderSize);
                    Point point1, point2;
                    double rotation = 0;

                    if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (DropInfo.InsertIndex == itemParent.Items.Count)
                        {
                            itemRect.Y += itemContainer.RenderSize.Height;
                        }

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.Right, itemRect.Y);
                    }
                    else
                    {
                        if (DropInfo.InsertIndex == itemParent.Items.Count)
                        {
                            itemRect.X += itemContainer.RenderSize.Width;
                        }

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.X, itemRect.Bottom);
                        rotation = 90;
                    }

                    drawingContext.DrawLine(m_Pen, point1, point2);
                    DrawTriangle(drawingContext, point1, rotation);
                    DrawTriangle(drawingContext, point2, 180 + rotation);
                }
            }
        }
        
        void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
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
            const int triangleSize = 3;

            m_Pen = new Pen(Brushes.Gray, 2);
			m_Pen.Freeze();

            LineSegment firstLine = new LineSegment(new Point(0, -triangleSize), false);
			firstLine.Freeze();
            LineSegment secondLine = new LineSegment(new Point(0, triangleSize), false);
			secondLine.Freeze();

            PathFigure figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
			figure.Segments.Add(firstLine);
			figure.Segments.Add(secondLine);
			figure.Freeze();

			m_Triangle = new PathGeometry();
			m_Triangle.Figures.Add(figure);
			m_Triangle.Freeze();
		}

        static Pen m_Pen;
		static PathGeometry m_Triangle;
    }
}
