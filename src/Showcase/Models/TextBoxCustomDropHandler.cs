using System;
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

        /// <inheritdoc />
        public void DropHint(IDropHintInfo dropHintInfo)
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
            var textBox = (TextBox)dropInfo.VisualTarget;

            if (dropInfo.Data is IDataObject dataObject)
            {
                if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    textBox.Text = dataObject.GetData(DataFormats.Text) as string ?? string.Empty;
                }
                else if (dataObject.GetDataPresent(DataFormats.FileDrop))
                {
                    // Note that you can have more than one file.
                    string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
                    textBox.Text = string.Join(Environment.NewLine, files);
                }
            }
            else
            {
                var realData = DefaultDropHandler.ExtractData(dropInfo.Data);
                textBox.Text = string.Join(", ", realData.OfType<object>().ToArray());
            }
        }
    }

    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        private readonly Pen _pen;
        private readonly Brush _brush;

        public DropTargetHighlightAdorner(UIElement adornedElement, IDropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
            this._pen = new Pen(Brushes.Tomato, 2);
            this._pen.Freeze();
            this._brush = new SolidColorBrush(Colors.Coral) { Opacity = 0.2 };
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
                drawingContext.DrawRectangle(this._brush, this._pen, bounds);
            }
        }
    }
}