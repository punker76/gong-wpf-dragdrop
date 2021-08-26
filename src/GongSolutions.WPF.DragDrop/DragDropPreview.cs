using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    internal class DragDropPreview : Popup
    {
        public DragDropPreview(UIElement rootElement, UIElement previewElement, Point translation, Point anchorPoint)
        {
            this.PlacementTarget = rootElement;
            this.Placement = PlacementMode.Relative;
            this.AllowsTransparency = true;
            this.Focusable = false;
            this.PopupAnimation = PopupAnimation.Fade;
            this.StaysOpen = true;
            this.HorizontalOffset = -9999;
            this.VerticalOffset = -9999;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;

            this.Child = previewElement;
            this.Translation = translation;
            this.AnchorPoint = anchorPoint;
        }

        public Point Translation { get; }

        public Point AnchorPoint { get; }

        internal void Move(Point point)
        {
            var translation = this.Translation;
            var translationX = point.X + translation.X;
            var translationY = point.Y + translation.Y;

            var renderSize = this.Child.RenderSize;

            if (renderSize.Width > 0 && renderSize.Height > 0)
            {
                var offsetX = renderSize.Width * -this.AnchorPoint.X;
                var offsetY = renderSize.Height * -this.AnchorPoint.Y;

                translationX += offsetX;
                translationY += offsetY;
            }

            this.SetCurrentValue(HorizontalOffsetProperty, translationX);
            this.SetCurrentValue(VerticalOffsetProperty, translationY);
        }

        protected override void OnOpened(EventArgs e)

        {
            base.OnOpened(e);

            if (PresentationSource.FromVisual(this.Child) is HwndSource hwndSource)
            {
                var windowHandle = hwndSource.Handle;
                var wsex = WindowStyleHelper.GetWindowStyleEx(windowHandle);

                wsex |= WindowStyleHelper.WS_EX.NOACTIVATE; // We don't want our this window to be activated
                wsex |= WindowStyleHelper.WS_EX.TRANSPARENT;

                WindowStyleHelper.SetWindowStyleEx(windowHandle, wsex);
            }
        }
    }
}