using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        public DropTargetHighlightAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
        }

        /// <summary>
        /// The background brush for the highlight rectangle for TreeViewItem. This can be overridden through
        /// <see cref="DragDrop.DropTargetHighlightBrushProperty"/>. The default value is <see cref="Brushes.Transparent"/>.
        /// </summary>
        public Brush Background { get; set; } = Brushes.Transparent;

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system.
        /// The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for
        /// later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var dropInfo = this.DropInfo;
            var visualTargetItem = dropInfo.VisualTargetItem;
            if (visualTargetItem != null)
            {
                var rect = Rect.Empty;

                var tvItem = visualTargetItem as TreeViewItem;
                if (tvItem != null && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
                {
                    var descendant = VisualTreeExtensions.GetVisibleDescendantBounds(tvItem);
                    var translatePoint = tvItem.TranslatePoint(new Point(), this.AdornedElement);
                    var itemRect = new Rect(translatePoint, tvItem.RenderSize);
                    descendant.Union(itemRect);
                    translatePoint.Offset(1, 0);
                    rect = new Rect(translatePoint, new Size(descendant.Width - translatePoint.X - 1, tvItem.ActualHeight));
                }

                if (rect.IsEmpty)
                {
                    var bounds = VisualTreeExtensions.GetVisibleDescendantBounds(visualTargetItem);
                    var location = visualTargetItem.TranslatePoint(bounds.Location, this.AdornedElement);
                    rect = new Rect(location, bounds.Size);
                }

                drawingContext.DrawRoundedRectangle(this.Background, this.Pen, rect, 2, 2);
            }
        }
    }
}