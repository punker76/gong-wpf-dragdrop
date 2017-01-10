using System;
using System.Windows.Documents;
using System.Windows;

namespace GongSolutions.Wpf.DragDrop
{
  public abstract class DropTargetAdorner : Adorner
  {
    [Obsolete("This constructor is obsolete and will be deleted in next major release.")]
    public DropTargetAdorner(UIElement adornedElement)
      : this(adornedElement, (DropInfo)null)
    {
    }

    public DropTargetAdorner(UIElement adornedElement, DropInfo dropInfo)
      : base(adornedElement)
    {
      this.DropInfo = dropInfo;
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

    internal static DropTargetAdorner Create(Type type, UIElement adornedElement, IDropInfo dropInfo)
    {
      if (!typeof(DropTargetAdorner).IsAssignableFrom(type)) {
        throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
      }
      return type.GetConstructor(new[] {typeof(UIElement), typeof(DropInfo)})?.Invoke(new object[] {adornedElement, dropInfo}) as DropTargetAdorner;
    }

    private readonly AdornerLayer m_AdornerLayer;
  }
}