using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public class DragDropPreview : Popup
    {
        public DragDropPreview(UIElement rootElement, UIElement previewElement, Point translation, Point anchorPoint, DragDropEffects effects = DragDropEffects.None)
        {
            this.PlacementTarget = rootElement;
            this.AllowsTransparency = true;
            this.Focusable = false;
            this.Placement = PlacementMode.Relative;
            this.PlacementTarget = rootElement;
            this.PopupAnimation = PopupAnimation.Fade;
            this.StaysOpen = true;
            this.HorizontalOffset = -9999;
            this.VerticalOffset = -9999;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;

            this.Translation = translation;
            this.AnchorPoint = anchorPoint;
            this.Effects = effects;
            this.Child = previewElement;
        }

        public Point Translation { get; }

        public Point AnchorPoint { get; }

        public DragDropEffects Effects { get; }

        internal void Move(Point point)
        {
            var translationX = point.X + this.Translation.X;
            var translationY = point.Y + this.Translation.Y;

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
                var wsex = NativeMethods.GetWindowStyleEx(windowHandle);

                wsex |= NativeMethods.WS_EX.NOACTIVATE; // We don't want our this window to be activated
                wsex |= NativeMethods.WS_EX.TRANSPARENT;

                NativeMethods.SetWindowStyleEx(windowHandle, wsex);
            }
        }
    }
}