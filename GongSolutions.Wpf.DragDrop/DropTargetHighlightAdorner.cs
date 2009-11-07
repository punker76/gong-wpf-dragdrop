using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
            if (DropInfo.VisualTargetItem != null)
            {
                Rect rect = new Rect(
                    DropInfo.VisualTargetItem.TranslatePoint(new Point(), AdornedElement),
                    VisualTreeHelper.GetDescendantBounds(DropInfo.VisualTargetItem).Size);
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }
    }
}
