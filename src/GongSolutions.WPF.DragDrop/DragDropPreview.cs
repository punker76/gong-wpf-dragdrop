using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    internal class DragDropPreview : Popup
    {
        public DragDropPreview(UIElement rootElement, UIElement previewElement, Point translation, Point anchorPoint)
        {
            this.PlacementTarget = rootElement;
            this.Placement = PlacementMode.Relative;
            this.AllowsTransparency = true;
            this.Focusable = false;
            this.PopupAnimation = PopupAnimation.Fade;
            this.StaysOpen = true;
            this.HorizontalOffset = -9999;
            this.VerticalOffset = -9999;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;

            this.Child = previewElement;
            this.Translation = translation;
            this.AnchorPoint = anchorPoint;
        }

        public DragDropPreview(UIElement rootElement, IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            this.PlacementTarget = rootElement;
            this.Placement = PlacementMode.Relative;
            this.AllowsTransparency = true;
            this.Focusable = false;
            this.PopupAnimation = PopupAnimation.Fade;
            this.StaysOpen = true;
            this.HorizontalOffset = -9999;
            this.VerticalOffset = -9999;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            this.DragInfo = dragInfo;
            this.Child = this.CreatePreviewPresenter(dragInfo, visualTarget, sender);
            this.Translation = DragDrop.GetDragAdornerTranslation(dragInfo.VisualSource);
            this.AnchorPoint = DragDrop.GetDragMouseAnchorPoint(dragInfo.VisualSource);
        }

        private IDragInfo DragInfo { get; }

        private Rect VisualSourceItemBounds { get; set; } = Rect.Empty;

        public Point Translation { get; }

        public Point AnchorPoint { get; }

        public bool UseDefaultDragAdorner { get; private set; }

        public static readonly DependencyProperty ItemTemplateProperty
            = DependencyProperty.Register(nameof(ItemTemplate),
                                          typeof(DataTemplate),
                                          typeof(DragDropPreview),
                                          new FrameworkPropertyMetadata((DataTemplate)null));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => this.SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty
            = DependencyProperty.Register(nameof(ItemTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(DragDropPreview),
                                          new FrameworkPropertyMetadata((DataTemplateSelector)null));

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(ItemTemplateSelectorProperty);
            set => this.SetValue(ItemTemplateSelectorProperty, value);
        }

        public void Move(Point point)
        {
            var translation = this.Translation;
            var translationX = point.X + translation.X;
            var translationY = point.Y + translation.Y;

            if (this.Child is not null)
            {
                var renderSize = this.Child.RenderSize;

                var renderSizeWidth = renderSize.Width;
                var renderSizeHeight = renderSize.Height;

                // Only set if the template contains a Canvas.
                if (!this.VisualSourceItemBounds.IsEmpty)
                {
                    renderSizeWidth = Math.Min(renderSizeWidth, this.VisualSourceItemBounds.Width);
                    renderSizeHeight = Math.Min(renderSizeHeight, this.VisualSourceItemBounds.Height);
                }

                if (renderSizeWidth > 0 && renderSizeHeight > 0)
                {
                    var offsetX = renderSizeWidth * -this.AnchorPoint.X;
                    var offsetY = renderSizeHeight * -this.AnchorPoint.Y;

                    translationX += offsetX;
                    translationY += offsetY;
                }
            }

            this.SetCurrentValue(HorizontalOffsetProperty, translationX);
            this.SetCurrentValue(VerticalOffsetProperty, translationY);
        }

        protected override void OnOpened(EventArgs e)

        {
            base.OnOpened(e);

            if (PresentationSource.FromVisual(this.Child) is HwndSource hwndSource)
            {
                var windowHandle = hwndSource.Handle;
                var wsex = WindowStyleHelper.GetWindowStyleEx(windowHandle);

                wsex |= WindowStyleHelper.WS_EX.NOACTIVATE; // We don't want our this window to be activated
                wsex |= WindowStyleHelper.WS_EX.TRANSPARENT;

                WindowStyleHelper.SetWindowStyleEx(windowHandle, wsex);
            }
        }

        public void UpdatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            if (this.UseDefaultDragAdorner)
            {
                return;
            }

            var visualSource = dragInfo.VisualSource;

            DataTemplate template = DragDrop.TryGetDropAdornerTemplate(visualTarget, sender) ?? DragDrop.TryGetDragAdornerTemplate(visualSource, sender);
            DataTemplateSelector templateSelector = DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender) ?? DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender);

            if (template is not null)
            {
                templateSelector = null;
            }

            this.SetCurrentValue(ItemTemplateProperty, template);
            this.SetCurrentValue(ItemTemplateSelectorProperty, templateSelector);
        }

        public UIElement CreatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo.VisualSource;

            DataTemplate template = DragDrop.TryGetDropAdornerTemplate(visualTarget, sender) ?? DragDrop.TryGetDragAdornerTemplate(visualSource, sender);
            DataTemplateSelector templateSelector = DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender) ?? DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender);

            var useDefaultDragAdorner = template is null && templateSelector is null && DragDrop.GetUseDefaultDragAdorner(visualSource);
            var useVisualSourceItemSizeForDragAdorner = dragInfo.VisualSourceItem != null && DragDrop.GetUseVisualSourceItemSizeForDragAdorner(visualSource);

            if (useDefaultDragAdorner)
            {
                template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
                useDefaultDragAdorner = template is not null;
            }

            if (template is not null)
            {
                templateSelector = null;
            }

            this.SetCurrentValue(ItemTemplateProperty, template);
            this.SetCurrentValue(ItemTemplateSelectorProperty, templateSelector);

            this.UseDefaultDragAdorner = useDefaultDragAdorner;

            UIElement adornment = null;

            if (template != null || templateSelector != null)
            {
                if (dragInfo.Data is IEnumerable items && !(items is string))
                {
                    var itemsCount = items.Cast<object>().Count();
                    var maxItemsCount = DragDrop.TryGetDragPreviewMaxItemsCount(dragInfo, sender);
                    if (!useDefaultDragAdorner && itemsCount <= maxItemsCount)
                    {
                        // sort items if necessary before creating the preview
                        var sorter = DragDrop.TryGetDragPreviewItemsSorter(dragInfo, sender);

                        var itemsControl = new ItemsControl
                                           {
                                               ItemsSource = sorter?.SortDragPreviewItems(items) ?? items,
                                               Tag = dragInfo
                                           };

                        itemsControl.SetBinding(ItemsControl.ItemTemplateProperty, new Binding(nameof(this.ItemTemplate)) { Source = this });
                        itemsControl.SetBinding(ItemsControl.ItemTemplateSelectorProperty, new Binding(nameof(this.ItemTemplateSelector)) { Source = this });

                        if (useVisualSourceItemSizeForDragAdorner)
                        {
                            var bounds = VisualTreeExtensions.GetVisibleDescendantBounds(dragInfo.VisualSourceItem);
                            itemsControl.SetCurrentValue(MinWidthProperty, bounds.Width);
                        }

                        // The ItemsControl doesn't display unless we create a grid to contain it.
                        var grid = new Grid();
                        grid.Children.Add(itemsControl);
                        adornment = grid;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter
                                           {
                                               Content = dragInfo.Data,
                                               Tag = dragInfo
                                           };

                    contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding(nameof(this.ItemTemplate)) { Source = this });
                    contentPresenter.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding(nameof(this.ItemTemplateSelector)) { Source = this });

                    if (useVisualSourceItemSizeForDragAdorner)
                    {
                        var bounds = VisualTreeExtensions.GetVisibleDescendantBounds(dragInfo.VisualSourceItem);
                        contentPresenter.SetCurrentValue(MinWidthProperty, bounds.Width);
                        contentPresenter.SetCurrentValue(MinHeightProperty, bounds.Height);
                    }

                    contentPresenter.Loaded += this.ContentPresenter_OnLoaded;

                    adornment = contentPresenter;
                }
            }

            if (adornment != null && useDefaultDragAdorner)
            {
                adornment.Opacity = DragDrop.GetDefaultDragAdornerOpacity(visualSource);
            }

            return adornment;
        }

        private void ContentPresenter_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ContentPresenter contentPresenter)
            {
                contentPresenter.Loaded -= this.ContentPresenter_OnLoaded;

                // If the template contains a Canvas then we get a strange size.
                if (this.UseDefaultDragAdorner && this.DragInfo?.VisualSourceItem.GetVisualDescendent<Canvas>() is not null)
                {
                    this.VisualSourceItemBounds = this.DragInfo?.VisualSourceItem != null ? VisualTreeHelper.GetDescendantBounds(this.DragInfo.VisualSourceItem) : Rect.Empty;

                    contentPresenter.SetCurrentValue(MaxWidthProperty, this.VisualSourceItemBounds.Width);
                    contentPresenter.SetCurrentValue(MaxHeightProperty, this.VisualSourceItemBounds.Height);
                    this.SetCurrentValue(MaxWidthProperty, this.VisualSourceItemBounds.Width);
                    this.SetCurrentValue(MaxHeightProperty, this.VisualSourceItemBounds.Height);
                }
                else
                {
                    contentPresenter.ApplyTemplate();
                    if (contentPresenter.GetVisualDescendent<Canvas>() is not null)
                    {
                        // Get the first element and set it's vertical alignment to top.
                        if (contentPresenter.GetVisualDescendent<DependencyObject>() is FrameworkElement fe)
                        {
                            fe.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Top);
                        }

                        this.VisualSourceItemBounds = this.DragInfo?.VisualSourceItem != null ? VisualTreeHelper.GetDescendantBounds(this.DragInfo.VisualSourceItem) : Rect.Empty;

                        contentPresenter.SetCurrentValue(MaxWidthProperty, this.VisualSourceItemBounds.Width);
                        contentPresenter.SetCurrentValue(MaxHeightProperty, this.VisualSourceItemBounds.Height);
                        this.SetCurrentValue(MaxWidthProperty, this.VisualSourceItemBounds.Width);
                        this.SetCurrentValue(MaxHeightProperty, this.VisualSourceItemBounds.Height);
                    }
                }
            }
        }
    }
}