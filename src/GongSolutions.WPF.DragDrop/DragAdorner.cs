using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    internal class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adornedElement, UIElement adornment, Point translation, DragDropEffects effects = DragDropEffects.None)
            : base(adornedElement)
        {
            this.Translation = translation;
            this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.m_AdornerLayer?.Add(this);
            this.m_Adornment = adornment;
            this.IsHitTestVisible = false;
            this.Effects = effects;
        }

        public Point Translation { get; private set; }

        public DragDropEffects Effects { get; private set; }

        public Point MousePosition
        {
            get { return this.m_MousePosition; }
            set
            {
                if (this.m_MousePosition != value)
                {
                    this.m_MousePosition = value;
                    this.m_AdornerLayer.Update(this.AdornedElement);
                }
            }
        }

        public void Detatch()
        {
            this.m_AdornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.m_Adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.MousePosition.X + this.Translation.X, this.MousePosition.Y + this.Translation.Y));

            return result;
        }

        internal void Move(Point newAdornerPosition, Point anchorPoint, ref Point adornerMousePosition, ref Size adornerSize)
        {
            if (newAdornerPosition.X >= 0 && newAdornerPosition.Y >= 0)
            {
                adornerMousePosition = newAdornerPosition;
            }

            if (this.RenderSize.Width > 0 && this.RenderSize.Height > 0)
            {
                adornerSize = this.RenderSize;
            }

            // flickering fix
            if (adornerSize.Width <= 0 || adornerSize.Height <= 0)
            {
                return;
            }

            var offsetX = adornerSize.Width * -anchorPoint.X;
            var offsetY = adornerSize.Height * -anchorPoint.Y;
            adornerMousePosition.Offset(offsetX, offsetY);

            if (adornerMousePosition.X < 0)
            {
                adornerMousePosition.X = 0;
            }
            else
            {
                var maxAdornerPosX = this.AdornedElement.RenderSize.Width;
                var adornerPosRightX = (adornerMousePosition.X + this.Translation.X + adornerSize.Width);
                if (adornerPosRightX > maxAdornerPosX)
                {
                    adornerMousePosition.Offset(-adornerPosRightX + maxAdornerPosX, 0);
                }
            }

            if (adornerMousePosition.Y < 0)
            {
                adornerMousePosition.Y = 0;
            }
            else
            {
                var maxAdornerPosY = this.AdornedElement.RenderSize.Height;
                var adornerPosRightY = (adornerMousePosition.Y + this.Translation.Y + adornerSize.Height);
                if (adornerPosRightY > maxAdornerPosY)
                {
                    adornerMousePosition.Offset(0, -adornerPosRightY + maxAdornerPosY);
                }
            }

            this.MousePosition = adornerMousePosition;
            this.InvalidateVisual();
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.m_Adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.m_Adornment.Measure(constraint);
            return this.m_Adornment.DesiredSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        private readonly AdornerLayer m_AdornerLayer;
        private readonly UIElement m_Adornment;
        private Point m_MousePosition;
    }
}