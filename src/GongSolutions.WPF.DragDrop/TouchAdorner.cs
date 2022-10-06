using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GongSolutions.Wpf.DragDrop
{
    public class TouchAdorner : Adorner
    {
        private readonly TouchPoint touchPoint;
        private readonly Brush background;
        private Ellipse ellipse;
        private readonly VisualCollection visualChildren;

        public TouchAdorner(UIElement adornedElement, TouchPoint touchPoint, Brush background) : base(adornedElement)
        {
            this.touchPoint = touchPoint;
            this.background = background;
            visualChildren = new VisualCollection(this);
            CreateChildren();
        }

        private void CreateChildren()
        {
            ellipse = new Ellipse
            {
                Fill = background,
                IsHitTestVisible = false,
                Opacity = 1.0,
                Width = 90,
                Height = 90
            };

            ellipse.Loaded += OnEllipseLoaded;

            visualChildren.Add(ellipse);
        }

        private void OnEllipseLoaded(object sender, RoutedEventArgs e)
        {
            StartAnimation();
        }

        private void StartAnimation()
        {
            var sb = new Storyboard();

            var st = new ScaleTransform();
            ellipse.RenderTransform = st;
            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            
            var dak = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(0)
            };
            Storyboard.SetTargetProperty(dak, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(dak, ellipse);
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)), new QuinticEase { EasingMode = EasingMode.EaseOut }));
            sb.Children.Add(dak);

            dak = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(0)
            };
            Storyboard.SetTargetProperty(dak, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(dak, ellipse);
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)), new QuinticEase { EasingMode = EasingMode.EaseOut }));
            sb.Children.Add(dak);

            dak = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(dak, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(dak, ellipse);
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)), new QuinticEase { EasingMode = EasingMode.EaseOut }));
            sb.Children.Add(dak);

            dak = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(dak, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(dak, ellipse);
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            dak.KeyFrames.Add(new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)), new QuinticEase { EasingMode = EasingMode.EaseOut }));
            sb.Children.Add(dak);

            sb.Begin();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var x = touchPoint.Position.X - ellipse.DesiredSize.Width / 2;
            var y = touchPoint.Position.Y - ellipse.DesiredSize.Height / 2;

            var rect = new Rect(x, y, ellipse.DesiredSize.Width, ellipse.DesiredSize.Height);

            Debug.WriteLine($"Arrange: {rect.X},{rect.Y},{rect.Width},{rect.Height}");

            ellipse.Arrange(rect);

            return finalSize;
        }

        protected override int VisualChildrenCount => visualChildren.Count;
        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }
    }
}