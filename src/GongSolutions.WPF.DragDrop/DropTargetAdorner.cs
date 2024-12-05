using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    using JetBrains.Annotations;

    public abstract class DropTargetAdorner : Adorner
    {
        public DropTargetAdorner(UIElement adornedElement, IDropInfo dropInfo)
            : base(adornedElement)
        {
            this.DropInfo = dropInfo;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.SnapsToDevicePixels = true;
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            // can be null but should normally not be null
            this.adornerLayer?.Add(this);
        }

        public IDropInfo DropInfo { get; set; }

        /// <summary>
        /// Gets or Sets the pen which can be used for the render process.
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);

        public void Detach()
        {
            if (this.adornerLayer is null)
            {
                return;
            }

            if (!this.adornerLayer.Dispatcher.CheckAccess())
            {
                this.adornerLayer.Dispatcher.Invoke(this.Detach);
                return;
            }

            this.adornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement, IDropInfo dropInfo)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
            }

            return type.GetConstructor(new[] { typeof(UIElement), typeof(DropInfo) })?.Invoke(new object[] { adornedElement, dropInfo }) as DropTargetAdorner;
        }

        [CanBeNull]
        private readonly AdornerLayer adornerLayer;
    }
}