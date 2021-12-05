using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class DragDropExtensions
    {
        /// <summary>
        /// Determines whether the given element is ignored on drag start (<see cref="DragDrop.DragSourceIgnoreProperty"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDragSourceIgnored(this UIElement element)
        {
            return element != null && DragDrop.GetDragSourceIgnore(element);
        }

        /// <summary>
        /// Determines whether the given element is ignored on drop action (<see cref="DragDrop.IsDragSourceProperty"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDragSource(this UIElement element)
        {
            return element != null && DragDrop.GetIsDragSource(element);
        }

        /// <summary>
        /// Determines whether the given element is ignored on drop action (<see cref="DragDrop.IsDropTargetProperty"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDropTarget(this UIElement element)
        {
            return element != null && DragDrop.GetIsDropTarget(element);
        }

        /// <summary>
        /// Gets if drop position is directly over element
        /// </summary>
        /// <param name="dropPosition">Drop position</param>
        /// <param name="element">element to check whether or not the drop position is directly over or not</param>
        /// <param name="relativeToElement">element to which the drop position is related</param>
        /// <returns>drop position is directly over element or not</returns>
        public static bool DirectlyOverElement(this Point dropPosition, UIElement element, UIElement relativeToElement)
        {
            if (element == null)
                return false;

            var relativeItemPosition = element.TranslatePoint(new Point(0, 0), relativeToElement);
            var relativeDropPosition = new Point(dropPosition.X - relativeItemPosition.X, dropPosition.Y - relativeItemPosition.Y);
            return VisualTreeExtensions.GetVisibleDescendantBounds(element).Contains(relativeDropPosition);
        }

        /// <summary>
        /// Capture screen and create data template containing the captured image
        /// </summary>
        /// <param name="element">visual source to capture screen of</param>
        /// <param name="visualSourceFlowDirection">Flowdirection of visual source</param>
        /// <returns></returns>
        public static DataTemplate GetCaptureScreenDataTemplate(this UIElement element, FlowDirection visualSourceFlowDirection)
        {
            DataTemplate template = null;
            var bs = CaptureScreen(element, visualSourceFlowDirection);
            if (bs != null)
            {
                var factory = new FrameworkElementFactory(typeof(Image));
                factory.SetValue(Image.SourceProperty, bs);
                factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                factory.SetValue(FrameworkElement.WidthProperty, bs.Width);
                factory.SetValue(FrameworkElement.HeightProperty, bs.Height);
                factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
                template = new DataTemplate { VisualTree = factory };
            }

            return template;
        }

        // Helper to generate the image - I grabbed this off Google 
        // somewhere. -- Chris Bordeman cbordeman@gmail.com
        private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection)
        {
            if (target == null)
            {
                return null;
            }

            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            var cropBounds = VisualTreeExtensions.GetVisibleDescendantBounds(target);

            var dpiScale = VisualTreeHelper.GetDpi(target);
            var dpiX = dpiScale.PixelsPerInchX;
            var dpiY = dpiScale.PixelsPerInchY;
            var dpiBounds = DpiHelper.LogicalRectToDevice(cropBounds, dpiScale.DpiScaleX, dpiScale.DpiScaleY);

            var pixelWidth = (int)Math.Ceiling(dpiBounds.Width);
            var pixelHeight = (int)Math.Ceiling(dpiBounds.Height);
            if (pixelWidth < 0 || pixelHeight < 0)
            {
                return null;
            }

            var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target);

                // vb.ViewportUnits = BrushMappingMode.Absolute;
                // vb.Viewport = bounds;

                if (flowDirection == FlowDirection.RightToLeft)
                {
                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(new ScaleTransform(-1, 1));
                    transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width, 0));
                    ctx.PushTransform(transformGroup);
                }

                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }
    }
}