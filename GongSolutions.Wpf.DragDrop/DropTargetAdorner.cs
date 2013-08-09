using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

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