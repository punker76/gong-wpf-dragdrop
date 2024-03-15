namespace GongSolutions.Wpf.DragDrop;

using System.Windows;
using System.Windows.Media;

/// <summary>
/// This adorner is used to display hints for where items can be dropped.
/// </summary>
public class DropTargetHintAdorner : DropTargetHighlightAdorner
{
    public DropTargetHintAdorner(UIElement adornedElement, DropInfo dropInfo)
        : base(adornedElement, dropInfo)
    {
        Background = new SolidColorBrush(Colors.Green) { Opacity = 0.2 };
        Background.Freeze();

        Pen = new Pen(Brushes.DarkGreen, 0.2);
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        var visualTarget = this.DropInfo.VisualTarget;
        if (visualTarget != null)
        {
            var translatePoint = visualTarget.TranslatePoint(new Point(), this.AdornedElement);
            translatePoint.Offset(1, 1);
            var bounds = new Rect(translatePoint, new Size(visualTarget.RenderSize.Width - 2, visualTarget.RenderSize.Height - 2));
            drawingContext.DrawRectangle(this.Background, this.Pen, bounds);
        }
    }
}