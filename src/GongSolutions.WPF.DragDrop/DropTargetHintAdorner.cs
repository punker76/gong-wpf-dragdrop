namespace GongSolutions.Wpf.DragDrop;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

/// <summary>
/// This adorner is used to display hints for where items can be dropped.
/// </summary>
public class DropTargetHintAdorner : Adorner
{
    private readonly ContentPresenter m_Presenter;
    private readonly AdornerLayer m_AdornerLayer;

    public static readonly DependencyProperty DropHintDataProperty = DependencyProperty.Register(
        nameof(DropHintData), typeof(DropHintData), typeof(DropTargetHintAdorner), new PropertyMetadata(default(DropHintData)));

    public DropHintData DropHintData
    {
        get => (DropHintData)GetValue(DropHintDataProperty);
        set => SetValue(DropHintDataProperty, value);
    }

    public DropTargetHintAdorner(UIElement adornedElement,
                                 DataTemplate dataTemplate,
                                 DropHintData dropHintData)
        : base(adornedElement)
    {
        SetCurrentValue(DropHintDataProperty, dropHintData);
        this.IsHitTestVisible = false;
        this.AllowDrop = false;
        this.SnapsToDevicePixels = true;
        this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
        this.m_AdornerLayer?.Add(this);

        this.m_Presenter = new ContentPresenter()
                           {
                               IsHitTestVisible = false,
                               ContentTemplate = dataTemplate
                           };
        var binding = new Binding(nameof(DropHintData))
                      {
                          Source = this,
                          Mode = BindingMode.OneWay
                      };
        this.m_Presenter.SetBinding(ContentPresenter.ContentProperty, binding);
    }

    /// <summary>
    /// Detach the adorner from it's adorner layer.
    /// </summary>
    public void Detatch()
    {
        this.m_AdornerLayer?.Remove(this);
    }

    /// <summary>
    /// Construct a new drop hint target adorner.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="adornedElement"></param>
    /// <param name="dataTemplate"></param>
    /// <param name="hintData"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static DropTargetHintAdorner CreateHintAdorner(Type type, UIElement adornedElement, DataTemplate dataTemplate, DropHintData hintData)
    {
        if (!typeof(DropTargetHintAdorner).IsAssignableFrom(type))
        {
            throw new InvalidOperationException("The requested adorner class does not derive from DropTargetHintAdorner.");
        }

        return type.GetConstructor(new[]
                                   {
                                       typeof(UIElement),
                                       typeof(DataTemplate),
                                       typeof(DropHintData)
                                   })
                   ?.Invoke(new object[]
                            {
                                adornedElement,
                                dataTemplate,
                                hintData
                            })
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
        this.m_Presenter.Measure(constraint);
        return this.m_Presenter.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var bounds = GetBounds(AdornedElement as FrameworkElement, AdornedElement);
        this.m_Presenter.Arrange(bounds);
        return bounds.Size;
    }

    protected override Visual GetVisualChild(int index)
    {
        return this.m_Presenter;
    }

    protected override int VisualChildrenCount
    {
        get { return 1; }
    }

    /// <summary>
    /// Update hint text and state for the adorner.
    /// </summary>
    /// <param name="hintData"></param>
    public void Update(DropHintData hintData)
    {
        var currentData = DropHintData;
        bool requiresUpdate = (hintData?.HintState != currentData?.HintState || hintData?.HintText != currentData?.HintText);
        SetCurrentValue(DropHintDataProperty, hintData);
        if(requiresUpdate)
        {
            this.m_AdornerLayer.Update();
        }
    }
}