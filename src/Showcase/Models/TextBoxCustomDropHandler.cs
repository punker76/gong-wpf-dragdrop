using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
    public class TextBoxCustomDropHandler : IDropTarget
    {
#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragEnter(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = typeof(DropTargetHighlightAdorner);
            dropInfo.Effects = DragDropEffects.Move;
        }

#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragLeave(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public void Drop(IDropInfo dropInfo)
        {
            var dataAsList = DefaultDropHandler.ExtractData(dropInfo.Data);
            ((TextBox)dropInfo.VisualTarget).Text = string.Join(", ", dataAsList.OfType<object>().ToArray());
        }
    }

    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        private readonly Pen _pen;
        private readonly Brush _brush;

        public DropTargetHighlightAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
            _pen = new Pen(Brushes.Tomato, 2);
            _pen.Freeze();
            _brush = new SolidColorBrush(Colors.Coral) { Opacity = 0.2 };
            this._brush.Freeze();

            this.SetValue(SnapsToDevicePixelsProperty, true);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var visualTarget = this.DropInfo.VisualTarget;
            if (visualTarget != null)
            {
                var translatePoint = visualTarget.TranslatePoint(new Point(), this.AdornedElement);
                translatePoint.Offset(1, 1);
                var bounds = new Rect(translatePoint,
                                      new Size(visualTarget.RenderSize.Width - 2, visualTarget.RenderSize.Height - 2));
                drawingContext.DrawRectangle(_brush, _pen, bounds);
            }
        }
    }
}