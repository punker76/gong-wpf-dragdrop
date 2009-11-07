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
            m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            m_AdornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        public void Detatch()
        {
            m_AdornerLayer.Remove(this);
        }

        public DropInfo DropInfo { get; set; }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }

            return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                .Invoke(new[] { adornedElement });
        }

        AdornerLayer m_AdornerLayer;
    }
}
