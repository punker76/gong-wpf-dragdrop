using JetBrains.Annotations;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// This adorner is used to display hints for where items can be dropped.
    /// </summary>
    public class DropTargetHintAdorner : Adorner
    {
        private readonly ContentPresenter presenter;
        [CanBeNull]
        private readonly AdornerLayer adornerLayer;

        public static readonly DependencyProperty DropHintDataProperty
            = DependencyProperty.Register(nameof(DropHintData),
                                          typeof(DropHintData),
                                          typeof(DropTargetHintAdorner),
                                          new PropertyMetadata(default(DropHintData)));

        public DropHintData DropHintData
        {
            get => (DropHintData)this.GetValue(DropHintDataProperty);
            set => this.SetValue(DropHintDataProperty, value);
        }

        public DropTargetHintAdorner(UIElement adornedElement, DataTemplate dataTemplate, DropHintData dropHintData)
            : base(adornedElement)
        {
            this.SetCurrentValue(DropHintDataProperty, dropHintData);
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.SnapsToDevicePixels = true;
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.adornerLayer?.Add(this);

            this.presenter = new ContentPresenter()
                             {
                                 IsHitTestVisible = false,
                                 ContentTemplate = dataTemplate
                             };
            var binding = new Binding(nameof(this.DropHintData))
                          {
                              Source = this,
                              Mode = BindingMode.OneWay
                          };
            this.presenter.SetBinding(ContentPresenter.ContentProperty, binding);
        }

        /// <summary>
        /// Detach the adorner from its adorner layer.
        /// </summary>
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

        private static Rect GetBounds(FrameworkElement element, UIElement visual)
        {
            return new Rect(
                element.TranslatePoint(new Point(0, 0), visual),
                element.TranslatePoint(new Point(element.ActualWidth, element.ActualHeight), visual));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.presenter.Measure(constraint);
            return this.presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = GetBounds(this.AdornedElement as FrameworkElement, this.AdornedElement);
            this.presenter.Arrange(bounds);
            return bounds.Size;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.presenter;
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
            var currentData = this.DropHintData;
            bool requiresUpdate = (hintData?.HintState != currentData?.HintState || hintData?.HintText != currentData?.HintText);
            this.SetCurrentValue(DropHintDataProperty, hintData);
            if (requiresUpdate)
            {
                this.adornerLayer?.Update();
            }
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

            return type.GetConstructor([typeof(UIElement), typeof(DataTemplate), typeof(DropHintData)])
                       ?.Invoke([adornedElement, dataTemplate, hintData])
                as DropTargetHintAdorner;
        }
    }
}