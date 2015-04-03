using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
  internal class DragAdorner : Adorner
  {
    public DragAdorner(UIElement adornedElement, UIElement adornment, DragDropEffects effects = DragDropEffects.None)
      : base(adornedElement)
    {
      this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      this.m_AdornerLayer.Add(this);
      this.m_Adornment = adornment;
      this.IsHitTestVisible = false;
      this.Effects = effects;
    }

    public DragDropEffects Effects { get; private set; }

    public Point MousePosition
    {
      get { return this.m_MousePosition; }
      set
      {
        if (this.m_MousePosition != value) {
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
      result.Children.Add(new TranslateTransform(this.MousePosition.X - 4, this.MousePosition.Y - 4));

      return result;
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