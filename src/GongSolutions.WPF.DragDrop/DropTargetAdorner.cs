using System;
using System.Windows.Documents;
using System.Windows;

namespace GongSolutions.Wpf.DragDrop
{
  public abstract class DropTargetAdorner : Adorner
  {
    public DropTargetAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      this.m_AdornerLayer.Add(this);
      this.IsHitTestVisible = false;
      this.AllowDrop = false;
      this.SnapsToDevicePixels = true;
    }

    public void Detatch()
    {
      this.m_AdornerLayer.Remove(this);
    }

    public DropInfo DropInfo { get; set; }

    internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
    {
      if (!typeof(DropTargetAdorner).IsAssignableFrom(type)) {
        throw new InvalidOperationException(
          "The requested adorner class does not derive from DropTargetAdorner.");
      }

      return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                                    .Invoke(new[] { adornedElement });
    }

    private readonly AdornerLayer m_AdornerLayer;
  }
}