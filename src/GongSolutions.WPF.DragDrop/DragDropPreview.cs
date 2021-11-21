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
        protected DragDropPreview(UIElement rootElement, UIElement previewElement, Point translation, Point anchorPoint)
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

        /// <summary>Identifies the <see cref="ItemTemplate"/> dependency property.</summary>
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

        /// <summary>Identifies the <see cref="ItemTemplateSelector"/> dependency property.</summary>
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

        /// <summary>Identifies the <see cref="ItemsPanel"/> dependency property.</summary>
        public static readonly DependencyProperty ItemsPanelProperty
            = DependencyProperty.Register(nameof(ItemsPanel),
                                          typeof(ItemsPanelTemplate),
                                          typeof(DragDropPreview),
                                          new PropertyMetadata(default(ItemsPanelTemplate)));

        public ItemsPanelTemplate ItemsPanel
        {
            get => (ItemsPanelTemplate)this.GetValue(ItemsPanelProperty);
            set => this.SetValue(ItemsPanelProperty, value);
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
                var wsEx = WindowStyleHelper.GetWindowStyleEx(windowHandle);

                wsEx |= WindowStyleHelper.WS_EX.NOACTIVATE; // We don't want our this window to be activated
                wsEx |= WindowStyleHelper.WS_EX.TRANSPARENT;

                WindowStyleHelper.SetWindowStyleEx(windowHandle, wsEx);
            }
        }

        private static bool IsMultiSelection(IDragInfo dragInfo)
        {
            return dragInfo?.Data is IEnumerable and not string;
        }

        public static bool HasDragDropPreview(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return false;
            }

            var isMultiSelection = IsMultiSelection(dragInfo);

            // Check for target template or template selector
            DataTemplate template = isMultiSelection
                ? DragDrop.TryGetDropAdornerMultiItemTemplate(visualTarget, sender) ?? DragDrop.TryGetDropAdornerTemplate(visualTarget, sender)
                : DragDrop.TryGetDropAdornerTemplate(visualTarget, sender);
            DataTemplateSelector templateSelector = isMultiSelection
                ? DragDrop.TryGetDropAdornerMultiItemTemplateSelector(visualTarget, sender) ?? DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender)
                : DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender);

            if (template is not null)
            {
                templateSelector = null;
            }

            // Check for source template or template selector if there is no target one
            if (template is null && templateSelector is null)
            {
                template = isMultiSelection
                    ? DragDrop.TryGetDragAdornerMultiItemTemplate(visualSource, sender) ?? DragDrop.TryGetDragAdornerTemplate(visualSource, sender)
                    : DragDrop.TryGetDragAdornerTemplate(visualSource, sender);
                templateSelector = isMultiSelection
                    ? DragDrop.TryGetDragAdornerMultiItemTemplateSelector(visualSource, sender) ?? DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender)
                    : DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender);

                var useDefaultDragAdorner = template is null && templateSelector is null && DragDrop.GetUseDefaultDragAdorner(visualSource);
                if (useDefaultDragAdorner)
                {
                    template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
                }

                if (template is not null)
                {
                    templateSelector = null;
                }
            }

            return template is not null || templateSelector is not null;
        }

        public void UpdatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return;
            }

            var isMultiSelection = IsMultiSelection(dragInfo);

            // Get target template or template selector
            DataTemplate template = isMultiSelection
                ? DragDrop.TryGetDropAdornerMultiItemTemplate(visualTarget, sender) ?? DragDrop.TryGetDropAdornerTemplate(visualTarget, sender)
                : DragDrop.TryGetDropAdornerTemplate(visualTarget, sender);
            DataTemplateSelector templateSelector = isMultiSelection
                ? DragDrop.TryGetDropAdornerMultiItemTemplateSelector(visualTarget, sender) ?? DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender)
                : DragDrop.TryGetDropAdornerTemplateSelector(visualTarget, sender);
            ItemsPanelTemplate itemsPanel = DragDrop.TryGetDropAdornerItemsPanel(visualTarget, sender);

            if (template is not null)
            {
                templateSelector = null;
            }

            // Get source template or template selector if there is no target one
            if (template is null && templateSelector is null)
            {
                template = isMultiSelection
                    ? DragDrop.TryGetDragAdornerMultiItemTemplate(visualSource, sender) ?? DragDrop.TryGetDragAdornerTemplate(visualSource, sender)
                    : DragDrop.TryGetDragAdornerTemplate(visualSource, sender);
                templateSelector = isMultiSelection
                    ? DragDrop.TryGetDragAdornerMultiItemTemplateSelector(visualSource, sender) ?? DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender)
                    : DragDrop.TryGetDragAdornerTemplateSelector(visualSource, sender);
                itemsPanel = DragDrop.TryGetDragAdornerItemsPanel(visualTarget, sender);

                this.UseDefaultDragAdorner = template is null && templateSelector is null && DragDrop.GetUseDefaultDragAdorner(visualSource);
                if (this.UseDefaultDragAdorner)
                {
                    template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
                    this.UseDefaultDragAdorner = template is not null;
                }

                if (template is not null)
                {
                    templateSelector = null;
                }
            }

            this.SetCurrentValue(ItemTemplateSelectorProperty, templateSelector);
            this.SetCurrentValue(ItemTemplateProperty, template);
            this.SetCurrentValue(ItemsPanelProperty, itemsPanel);
        }

        private UIElement CreatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return null;
            }

            var useVisualSourceItemSizeForDragAdorner = dragInfo.VisualSourceItem != null && DragDrop.GetUseVisualSourceItemSizeForDragAdorner(visualSource);

            this.UpdatePreviewPresenter(dragInfo, visualTarget, sender);

            UIElement adornment = null;

            if (this.ItemTemplate != null || this.ItemTemplateSelector != null)
            {
                if (dragInfo.Data is IEnumerable enumerable and not string)
                {
                    var items = enumerable.Cast<object>().ToList();
                    var itemsCount = items.Count;
                    var maxItemsCount = DragDrop.TryGetDragPreviewMaxItemsCount(dragInfo, sender);
                    if (!this.UseDefaultDragAdorner && itemsCount <= maxItemsCount)
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
                        itemsControl.SetBinding(ItemsControl.ItemsPanelProperty, new Binding(nameof(this.ItemsPanel)) { Source = this });

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

            if (adornment != null && this.UseDefaultDragAdorner)
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