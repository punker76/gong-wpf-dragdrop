namespace GongSolutions.Wpf.DragDrop;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// This adorner is used to display hints for where items can be dropped.
/// </summary>
public class DropTargetHintAdorner : DropTargetHighlightAdorner
{
    private readonly IDropHintInfo _dropHintInfo;
    private readonly ContentPresenter _presenter;

    public DropTargetHintAdorner(UIElement adornedElement,
                                 IDropHintInfo dropHintInfo,
                                 DataTemplate dataTemplate)
        : base(adornedElement, dropHintInfo.DropInfo)
    {

        Pen = new Pen(Brushes.Green, 0.3);
        Background = new SolidColorBrush(Colors.Green) { Opacity = 0.5 };

        _dropHintInfo = dropHintInfo;

        // Not showing hint when no data template is provided
        _presenter = dataTemplate == null ? new ContentPresenter() :
            new ContentPresenter
            {
                Content = dropHintInfo.DestinationText,
                ContentTemplate = dataTemplate
            };

        if(dataTemplate != null)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            Pen = new Pen(new SolidColorBrush(Colors.Transparent), 0.5);
        }

        Background.Freeze();
        Pen.Freeze();
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (this.AdornedElement is UIElement visualTarget)
        {
            var translatePoint = visualTarget.TranslatePoint(new Point(), this.AdornedElement);
            translatePoint.Offset(1, 1);
            var width = Math.Max(visualTarget.RenderSize.Width, 2) - 2;
            var height = Math.Max(visualTarget.RenderSize.Width, 2) - 2;

            var bounds = new Rect(translatePoint, new Size(width, height));
            drawingContext.DrawRectangle(this.Background, this.Pen, bounds);
        }
    }

    internal static DropTargetHintAdorner CreateHintAdorner(Type type, UIElement adornedElement, IDropHintInfo dropHintInfo, DataTemplate dataTemplate)
    {
        if (!typeof(DropTargetHintAdorner).IsAssignableFrom(type))
        {
            throw new InvalidOperationException("The requested adorner class does not derive from DropTargetHintAdorner.");
        }

        return type.GetConstructor(new[]
                                   {
                                       typeof(UIElement),
                                       typeof(DropHintInfo),
                                       typeof(DataTemplate)
                                   })
                   ?.Invoke(new object[]
                            { adornedElement,
                                dropHintInfo,
                                dataTemplate})
            as DropTargetHintAdorner;
    }

    private static Rect GetBounds(FrameworkElement element, UIElement visual)
    {
        return new Rect(
            element.TranslatePoint(new Point(0, 0), visual),
            element.TranslatePoint(new Point(element.ActualWidth, element.ActualHeight), visual));
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _presenter.Measure(constraint);
        return _presenter.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var bounds = GetBounds(AdornedElement as FrameworkElement, AdornedElement);
        _presenter.Arrange(bounds);
        return bounds.Size;
    }

    protected override Visual GetVisualChild(int index)
    {
        return _presenter;
    }

    protected override int VisualChildrenCount
    {
        get { return 1; }
    }

    public ContentPresenter Presenter
    {
        get { return _presenter; }
    }
}