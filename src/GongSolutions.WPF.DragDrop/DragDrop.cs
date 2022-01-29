using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop.Icons;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public static partial class DragDrop
    {
        /// <summary>
        /// Gets the drag handler from the drag info or from the sender, if the drag info is null
        /// </summary>
        /// <param name="dragInfo">the drag info object</param>
        /// <param name="sender">the sender from an event, e.g. mouse down, mouse move</param>
        /// <returns></returns>
        private static IDragSource TryGetDragHandler(IDragInfo dragInfo, UIElement sender)
        {
            var dragHandler = (dragInfo?.VisualSource != null ? GetDragHandler(dragInfo.VisualSource) : null) ?? (sender != null ? GetDragHandler(sender) : null);

            return dragHandler ?? DefaultDragHandler;
        }

        /// <summary>
        /// Gets the drop handler from the drop info or from the sender, if the drop info is null
        /// </summary>
        /// <param name="dropInfo">the drop info object</param>
        /// <param name="sender">the sender from an event, e.g. drag over</param>
        /// <returns></returns>
        private static IDropTarget TryGetDropHandler(IDropInfo dropInfo, UIElement sender)
        {
            var dropHandler = (dropInfo?.VisualTarget != null ? GetDropHandler(dropInfo.VisualTarget) : null) ?? (sender != null ? GetDropHandler(sender) : null);

            return dropHandler ?? DefaultDropHandler;
        }

        /// <summary>
        /// Gets the drag info builder from the sender.
        /// </summary>
        /// <param name="sender">the sender from an event, e.g. drag over</param>
        /// <returns></returns>
        private static IDragInfoBuilder TryGetDragInfoBuilder(DependencyObject sender)
        {
            return sender != null ? GetDragInfoBuilder(sender) : null;
        }

        /// <summary>
        /// Gets the drop info builder from the sender.
        /// </summary>
        /// <param name="sender">the sender from an event, e.g. drag over</param>
        /// <returns></returns>
        private static IDropInfoBuilder TryGetDropInfoBuilder(DependencyObject sender)
        {
            return sender != null ? GetDropInfoBuilder(sender) : null;
        }

        /// <summary>
        /// Gets the RootElementFinder from the sender or uses the default implementation, if it's null.
        /// </summary>
        /// <param name="sender">the sender from an event, e.g. drag over</param>
        /// <returns></returns>
        private static IRootElementFinder TryGetRootElementFinder(UIElement sender)
        {
            var rootElementFinder = sender != null ? GetRootElementFinder(sender) : null;

            return rootElementFinder ?? DefaultRootElementFinder;
        }

        internal static DataTemplate TryGetDragAdornerTemplate(UIElement source, UIElement sender)
        {
            var template = source is not null ? GetDragAdornerTemplate(source) : null;
            if (template is null && sender is not null)
            {
                template = GetDragAdornerTemplate(sender);
            }

            return template;
        }

        internal static DataTemplateSelector TryGetDragAdornerTemplateSelector(UIElement source, UIElement sender)
        {
            var templateSelector = source is not null ? GetDragAdornerTemplateSelector(source) : null;
            if (templateSelector is null && sender is not null)
            {
                templateSelector = GetDragAdornerTemplateSelector(sender);
            }

            return templateSelector;
        }

        internal static DataTemplate TryGetDragAdornerMultiItemTemplate(UIElement source, UIElement sender)
        {
            var template = source is not null ? GetDragAdornerMultiItemTemplate(source) : null;
            if (template is null && sender is not null)
            {
                template = GetDragAdornerMultiItemTemplate(sender);
            }

            return template;
        }

        internal static DataTemplateSelector TryGetDragAdornerMultiItemTemplateSelector(UIElement source, UIElement sender)
        {
            var templateSelector = source is not null ? GetDragAdornerMultiItemTemplateSelector(source) : null;
            if (templateSelector is null && sender is not null)
            {
                templateSelector = GetDragAdornerMultiItemTemplateSelector(sender);
            }

            return templateSelector;
        }

        internal static ItemsPanelTemplate TryGetDragAdornerItemsPanel(UIElement source, UIElement sender)
        {
            var itemsPanel = source is not null ? GetDragAdornerItemsPanel(source) : null;
            if (itemsPanel is null && sender is not null)
            {
                itemsPanel = GetDragAdornerItemsPanel(sender);
            }

            return itemsPanel;
        }

        internal static DataTemplate TryGetDropAdornerTemplate(UIElement source, UIElement sender)
        {
            var template = source is not null ? GetDropAdornerTemplate(source) : null;
            if (template is null && sender is not null)
            {
                template = GetDropAdornerTemplate(sender);
            }

            return template;
        }

        internal static DataTemplateSelector TryGetDropAdornerTemplateSelector(UIElement source, UIElement sender)
        {
            var templateSelector = source is not null ? GetDropAdornerTemplateSelector(source) : null;
            if (templateSelector is null && sender is not null)
            {
                templateSelector = GetDropAdornerTemplateSelector(sender);
            }

            return templateSelector;
        }

        internal static DataTemplate TryGetDropAdornerMultiItemTemplate(UIElement source, UIElement sender)
        {
            var template = source is not null ? GetDropAdornerMultiItemTemplate(source) : null;
            if (template is null && sender is not null)
            {
                template = GetDropAdornerMultiItemTemplate(sender);
            }

            return template;
        }

        internal static DataTemplateSelector TryGetDropAdornerMultiItemTemplateSelector(UIElement source, UIElement sender)
        {
            var templateSelector = source is not null ? GetDropAdornerMultiItemTemplateSelector(source) : null;
            if (templateSelector is null && sender is not null)
            {
                templateSelector = GetDropAdornerMultiItemTemplateSelector(sender);
            }

            return templateSelector;
        }

        internal static ItemsPanelTemplate TryGetDropAdornerItemsPanel(UIElement source, UIElement sender)
        {
            var itemsPanel = source is not null ? GetDropAdornerItemsPanel(source) : null;
            if (itemsPanel is null && sender is not null)
            {
                itemsPanel = GetDropAdornerItemsPanel(sender);
            }

            return itemsPanel;
        }

        internal static int TryGetDragPreviewMaxItemsCount(IDragInfo dragInfo, UIElement sender)
        {
            var itemsCount = dragInfo?.VisualSource != null ? GetDragPreviewMaxItemsCount(dragInfo.VisualSource) : -1;
            if (itemsCount < 0 && sender != null)
            {
                itemsCount = GetDragPreviewMaxItemsCount(sender);
            }

            return itemsCount < 0 || itemsCount >= int.MaxValue ? 10 : itemsCount;
        }

        internal static IDragPreviewItemsSorter TryGetDragPreviewItemsSorter(IDragInfo dragInfo, UIElement sender)
        {
            var itemsSorter = dragInfo?.VisualSource != null ? GetDragPreviewItemsSorter(dragInfo.VisualSource) : null;
            if (itemsSorter is null && sender != null)
            {
                itemsSorter = GetDragPreviewItemsSorter(sender);
            }

            return itemsSorter;
        }

        private static IDropTargetItemsSorter TryGetDropTargetItemsSorter(IDropInfo dropInfo, UIElement sender)
        {
            var itemsSorter = dropInfo?.VisualTarget != null ? GetDropTargetItemsSorter(dropInfo.VisualTarget) : null;
            if (itemsSorter is null && sender != null)
            {
                itemsSorter = GetDropTargetItemsSorter(sender);
            }

            return itemsSorter;
        }

        private static DragDropPreview GetDragDropPreview(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return null;
            }

            var hasDragDropPreview = DragDropPreview.HasDragDropPreview(dragInfo, visualTarget ?? visualSource, sender);
            if (hasDragDropPreview)
            {
                var rootElement = TryGetRootElementFinder(sender).FindRoot(visualTarget ?? visualSource);

                var preview = new DragDropPreview(rootElement, dragInfo, visualTarget ?? visualSource, sender);
                if (preview.Child != null)
                {
                    preview.IsOpen = true;
                    return preview;
                }
            }

            return null;
        }

        private static DragDropEffectPreview GetDragDropEffectPreview(IDropInfo dropInfo, UIElement sender)
        {
            var dragInfo = dropInfo.DragInfo;
            var template = GetDragDropEffectTemplate(dragInfo.VisualSource, dropInfo);

            if (template != null)
            {
                var rootElement = TryGetRootElementFinder(sender).FindRoot(dropInfo.VisualTarget ?? dragInfo.VisualSource);

                var adornment = new ContentPresenter { Content = dragInfo.Data, ContentTemplate = template };

                var preview = new DragDropEffectPreview(rootElement, adornment, GetEffectAdornerTranslation(dragInfo.VisualSource), dropInfo.Effects, dropInfo.EffectText, dropInfo.DestinationText)
                              {
                                  IsOpen = true
                              };

                return preview;
            }

            return null;
        }

        private static DataTemplate GetDragDropEffectTemplate(UIElement target, IDropInfo dropInfo)
        {
            if (target is null)
            {
                return null;
            }

            var effectText = dropInfo.EffectText;
            var destinationText = dropInfo.DestinationText;

            return dropInfo.Effects switch
            {
                DragDropEffects.All => GetEffectAllAdornerTemplate(target), // TODO: Add default template for EffectAll
                DragDropEffects.Copy => GetEffectCopyAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectCopy, string.IsNullOrEmpty(effectText) ? "Copy to" : effectText, destinationText),
                DragDropEffects.Link => GetEffectLinkAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectLink, string.IsNullOrEmpty(effectText) ? "Link to" : effectText, destinationText),
                DragDropEffects.Move => GetEffectMoveAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectMove, string.IsNullOrEmpty(effectText) ? "Move to" : effectText, destinationText),
                DragDropEffects.None => GetEffectNoneAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectNone, string.IsNullOrEmpty(effectText) ? "None" : effectText, destinationText),
                DragDropEffects.Scroll => GetEffectScrollAdornerTemplate(target), // TODO: Add default template EffectScroll
                _ => null
            };
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(UIElement target, BitmapImage effectIcon, string effectText, string destinationText)
        {
            if (!GetUseDefaultEffectDataTemplate(target))
            {
                return null;
            }

            var fontSize = SystemFonts.MessageFontSize; // before 11d

            // The icon
            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetValue(Image.SourceProperty, effectIcon);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 12d);
            imageFactory.SetValue(FrameworkElement.WidthProperty, 12d);

            // Only the icon for no effect
            if (Equals(effectIcon, GongSolutions.Wpf.DragDrop.Icons.IconFactory.EffectNone))
            {
                return new DataTemplate { VisualTree = imageFactory };
            }

            // Some margin for the icon
            imageFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 3, 0));
            imageFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Add effect text
            var effectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            effectTextBlockFactory.SetValue(TextBlock.TextProperty, effectText);
            effectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, fontSize);
            effectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);
            effectTextBlockFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Add destination text
            var destinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            destinationTextBlockFactory.SetValue(TextBlock.TextProperty, destinationText);
            destinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, fontSize);
            destinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            destinationTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3, 0, 0, 0));
            destinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);
            destinationTextBlockFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Create containing panel
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2));
            stackPanelFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            stackPanelFactory.AppendChild(imageFactory);
            stackPanelFactory.AppendChild(effectTextBlockFactory);
            stackPanelFactory.AppendChild(destinationTextBlockFactory);

            // Add border
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            var stopCollection = new GradientStopCollection
                                 {
                                     new GradientStop(Colors.White, 0.0),
                                     new GradientStop(Colors.AliceBlue, 1.0)
                                 };
            var gradientBrush = new LinearGradientBrush(stopCollection)
                                {
                                    StartPoint = new Point(0, 0),
                                    EndPoint = new Point(0, 1)
                                };
            borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            borderFactory.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            borderFactory.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
            borderFactory.SetValue(TextOptions.TextHintingModeProperty, TextHintingMode.Fixed);
            borderFactory.AppendChild(stackPanelFactory);

            // Finally add content to template
            return new DataTemplate { VisualTree = borderFactory };
        }

        private static void Scroll(IDropInfo dropInfo, DragEventArgs e)
        {
            if (dropInfo?.TargetScrollViewer is null)
            {
                return;
            }

            var scrollViewer = dropInfo.TargetScrollViewer;
            var scrollingMode = dropInfo.TargetScrollingMode;

            var position = e.GetPosition(scrollViewer);
            var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

            if (scrollingMode == ScrollingMode.Both || scrollingMode == ScrollingMode.HorizontalOnly)
            {
                if (position.X >= scrollViewer.ActualWidth - scrollMargin && scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
            }

            if (scrollingMode == ScrollingMode.Both || scrollingMode == ScrollingMode.VerticalOnly)
            {
                if (position.Y >= scrollViewer.ActualHeight - scrollMargin && scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }

        private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        private static void DragSourceOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        private static void DragSourceOnTouchDown(object sender, TouchEventArgs e)
        {
            _dragInfo = null;

            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            var elementPosition = e.GetTouchPoint((IInputElement)sender).Position;
            if ((sender as UIElement).IsDragSourceIgnored()
                || (e.Source as UIElement).IsDragSourceIgnored()
                || (e.OriginalSource as UIElement).IsDragSourceIgnored()
                || GetHitTestResult(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e))
            {
                return;
            }

            var infoBuilder = TryGetDragInfoBuilder(sender as DependencyObject);
            var dragInfo = infoBuilder?.CreateDragInfo(sender, e.OriginalSource, MouseButton.Left, item => e.GetTouchPoint(item).Position)
                           ?? new DragInfo(sender, e.OriginalSource, MouseButton.Left, item => e.GetTouchPoint(item).Position);

            DragSourceDown(sender, dragInfo, e, elementPosition);
        }

        private static void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragInfo = null;

            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1
                || (sender as UIElement).IsDragSourceIgnored()
                || (e.Source as UIElement).IsDragSourceIgnored()
                || (e.OriginalSource as UIElement).IsDragSourceIgnored()
                || GetHitTestResult(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e))
            {
                return;
            }

            var infoBuilder = TryGetDragInfoBuilder(sender as DependencyObject);
            var dragInfo = infoBuilder?.CreateDragInfo(sender, e.OriginalSource, e.ChangedButton, item => e.GetPosition(item))
                           ?? new DragInfo(sender, e.OriginalSource, e.ChangedButton, item => e.GetPosition(item));

            DragSourceDown(sender, dragInfo, e, elementPosition);
        }

        private static void DragSourceDown(object sender, DragInfo dragInfo, InputEventArgs e, Point elementPosition)
        {
            if (dragInfo.VisualSource is ItemsControl control && control.CanSelectMultipleItems())
            {
                control.Focus();
            }

            if (dragInfo.VisualSourceItem == null)
            {
                return;
            }

            var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
            if (!dragHandler.CanStartDrag(dragInfo))
            {
                return;
            }

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0
                && (Keyboard.Modifiers & ModifierKeys.Control) == 0
                && dragInfo.VisualSourceItem != null
                && sender is ItemsControl itemsControl
                && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().OfType<object>().ToList();
                if (selectedItems.Count > 1 && selectedItems.Contains(dragInfo.SourceItem))
                {
                    if (!HitTestUtilities.HitTest4Type<ToggleButton>(sender, elementPosition))
                    {
                        _clickSupressItem = dragInfo.SourceItem;
                        e.Handled = true;
                    }
                }
            }

            _dragInfo = dragInfo;
        }

        private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragSourceUp(sender, e.GetPosition((IInputElement)sender));
        }

        private static void DragSourceOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragSourceUp(sender, e.GetPosition((IInputElement)sender));
        }

        private static void DragSourceOnTouchUp(object sender, TouchEventArgs e)
        {
            DragSourceUp(sender, e.GetTouchPoint((IInputElement)sender).Position);
        }

        private static void DragSourceUp(object sender, Point elementPosition)
        {
            if (HitTestUtilities.HitTest4Type<ToggleButton>(sender, elementPosition))
            {
                return;
            }

            var dragInfo = _dragInfo;

            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            if (dragInfo?.VisualSource is ItemsControl itemsControl && _clickSupressItem != null && _clickSupressItem == dragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 || itemsControl is ListBox listBox && listBox.SelectionMode == SelectionMode.Multiple)
                {
                    itemsControl.SetItemSelected(dragInfo.SourceItem, false);
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    itemsControl.SetSelectedItem(dragInfo.SourceItem);

                    if (sender != itemsControl && sender is ItemsControl ancestorItemsControl)
                    {
                        var ancestorItemContainer = ancestorItemsControl.ContainerFromElement(itemsControl);

                        if (ancestorItemContainer != null)
                        {
                            var ancestorItem = ancestorItemsControl.ItemContainerGenerator.ItemFromContainer(ancestorItemContainer);

                            if (ancestorItem != null)
                            {
                                ancestorItemsControl.SetSelectedItem(ancestorItem);
                            }
                        }
                    }
                }
            }

            _dragInfo = null;
            _clickSupressItem = null;
        }

        private static void DragSourceOnTouchMove(object sender, TouchEventArgs e)
        {
            if (_dragInfo != null && !_dragInProgress)
            {
                // do nothing if mouse left/right button is released or the pointer is captured
                if (_dragInfo.MouseButton == MouseButton.Left && !e.TouchDevice.IsActive)
                {
                    _dragInfo = null;
                    return;
                }

                DoDragSourceMove(sender, element => e.GetTouchPoint(element).Position);
            }
        }

        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragInfo != null && !_dragInProgress)
            {
                if (_dragInfo.MouseButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
                {
                    _dragInfo = null;
                    return;
                }

                if (GetCanDragWithMouseRightButton(_dragInfo.VisualSource)
                    && _dragInfo.MouseButton == MouseButton.Right
                    && e.RightButton == MouseButtonState.Released)
                {
                    _dragInfo = null;
                    return;
                }

                DoDragSourceMove(sender, element => e.GetPosition(element));
            }
        }

        private static void DoDragSourceMove(object sender, Func<IInputElement, Point> getPosition)
        {
            var dragInfo = _dragInfo;
            if (dragInfo != null && !_dragInProgress)
            {
                // the start from the source
                var dragStart = dragInfo.DragStartPosition;

                // prevent selection changing while drag operation
                dragInfo.VisualSource?.ReleaseMouseCapture();

                // only if the sender is the source control and the mouse point differs from an offset
                var position = getPosition((IInputElement)sender);
                if (dragInfo.VisualSource == sender
                    && (Math.Abs(position.X - dragStart.X) > DragDrop.GetMinimumHorizontalDragDistance(dragInfo.VisualSource) ||
                        Math.Abs(position.Y - dragStart.Y) > DragDrop.GetMinimumVerticalDragDistance(dragInfo.VisualSource)))
                {
                    dragInfo.RefreshSelectedItems(sender);

                    var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
                    if (dragHandler.CanStartDrag(dragInfo))
                    {
                        dragHandler.StartDrag(dragInfo);

                        if (dragInfo.Effects != DragDropEffects.None)
                        {
                            var dataObject = dragInfo.DataObject;

                            if (dataObject == null)
                            {
                                if (dragInfo.Data == null)
                                {
                                    // it's bad if the Data is null, cause the DataObject constructor will raise an ArgumentNullException
                                    _dragInfo = null; // maybe not necessary or should not set here to null
                                    return;
                                }

                                dataObject = new DataObject(dragInfo.DataFormat.Name, dragInfo.Data);
                            }

                            var hookId = IntPtr.Zero;

                            try
                            {
                                _dragInProgress = true;

                                if (DragDropPreview is null)
                                {
                                    DragDropPreview = GetDragDropPreview(dragInfo, null, sender as UIElement);
                                    DragDropPreview?.Move(getPosition(DragDropPreview.PlacementTarget));
                                }

                                hookId = MouseHelper.HookMouseMove(point =>
                                    {
                                        DragDropPreview?.Move(CursorHelper.GetCurrentCursorPosition(DragDropPreview.PlacementTarget, point));
                                        DragDropEffectPreview?.Move(CursorHelper.GetCurrentCursorPosition(DragDropEffectPreview.PlacementTarget, point));
                                    });

                                var dragDropHandler = dragInfo.DragDropHandler ?? System.Windows.DragDrop.DoDragDrop;
                                var dragDropEffects = dragDropHandler(dragInfo.VisualSource, dataObject, dragInfo.Effects);
                                if (dragDropEffects == DragDropEffects.None)
                                {
                                    dragHandler.DragCancelled();
                                }

                                dragHandler.DragDropOperationFinished(dragDropEffects, dragInfo);
                            }
                            catch (Exception ex)
                            {
                                if (!dragHandler.TryCatchOccurredException(ex))
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                MouseHelper.RemoveHook(hookId);
                                _dragInProgress = false;
                                _dragInfo = null;
                            }
                        }
                    }
                }
            }
        }

        private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed || (e.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton) == e.KeyStates.HasFlag(DragDropKeyStates.RightMouseButton)))
            {
                DragDropPreview = null;
                DragDropEffectPreview = null;
                DropTargetAdorner = null;
                Mouse.OverrideCursor = null;
            }
        }

        private static void DropTargetOnDragLeave(object sender, DragEventArgs e)
        {
            SetIsDragOver(sender as DependencyObject, false);
            DropTargetAdorner = null;

            (sender as UIElement)?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (GetIsDragOver(sender as DependencyObject) == false && GetIsDragLeaved(sender as DependencyObject) == false)
                    {
                        OnRealTargetDragLeave(sender, e);
                    }
                }));
        }

        private static void OnRealTargetDragLeave(object sender, DragEventArgs e)
        {
            SetIsDragLeaved(sender as DependencyObject, true);

            var eventType = e.RoutedEvent?.RoutingStrategy switch
            {
                RoutingStrategy.Tunnel => EventType.Tunneled,
                RoutingStrategy.Bubble => EventType.Bubbled,
                _ => EventType.Auto
            };

            var dragInfo = _dragInfo;
            var dropInfoBuilder = TryGetDropInfoBuilder(sender as DependencyObject);
            var dropInfo = dropInfoBuilder?.CreateDropInfo(sender, e, dragInfo, eventType) ?? new DropInfo(sender, e, dragInfo, eventType);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);

            dropHandler?.DragLeave(dropInfo);

            DragDropEffectPreview = null;
            DropTargetAdorner = null;
        }

        private static void DropTargetOnDragEnter(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Bubbled, GetIsDragLeaved(sender as DependencyObject));
        }

        private static void DropTargetOnPreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Tunneled, GetIsDragLeaved(sender as DependencyObject));
        }

        private static void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Bubbled, false);
        }

        private static void DropTargetOnPreviewDragOver(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Tunneled, false);
        }

        private static void DropTargetOnDragOver(object sender, DragEventArgs e, EventType eventType, bool isDragEnter)
        {
            SetIsDragOver(sender as DependencyObject, true);
            SetIsDragLeaved(sender as DependencyObject, false);

            var elementPosition = e.GetPosition((IInputElement)sender);

            var dragInfo = _dragInfo;
            var dropInfoBuilder = TryGetDropInfoBuilder(sender as DependencyObject);
            var dropInfo = dropInfoBuilder?.CreateDropInfo(sender, e, dragInfo, eventType) ?? new DropInfo(sender, e, dragInfo, eventType);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
            var itemsControl = dropInfo.VisualTarget;

            if (isDragEnter)
            {
                dropHandler.DragEnter(dropInfo);
            }

            dropHandler.DragOver(dropInfo);

            if (dragInfo is not null)
            {
                if (DragDropPreview is null)
                {
                    DragDropPreview = GetDragDropPreview(dragInfo, dropInfo.VisualTarget, sender as UIElement);
                    DragDropPreview?.Move(e.GetPosition(DragDropPreview.PlacementTarget));
                }

                DragDropPreview?.UpdatePreviewPresenter(dragInfo, dropInfo.VisualTarget, sender as UIElement);
            }

            Scroll(dropInfo, e);

            if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (itemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                UIElement adornedElement;
                if (itemsControl is TabControl)
                {
                    adornedElement = itemsControl.GetVisualDescendent<TabPanel>();
                }
                else if (itemsControl is DataGrid || (itemsControl as ListView)?.View is GridView)
                {
                    adornedElement = itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement ?? itemsControl.GetVisualDescendent<ItemsPresenter>() as UIElement ?? itemsControl;
                }
                else
                {
                    adornedElement = itemsControl.GetVisualDescendent<ItemsPresenter>() as UIElement ?? itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement ?? itemsControl;
                }

                if (adornedElement != null)
                {
                    if (dropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement, dropInfo);
                    }

                    var adorner = DropTargetAdorner;
                    if (adorner != null)
                    {
                        var adornerPen = GetDropTargetAdornerPen(dropInfo.VisualTarget);
                        if (adornerPen != null)
                        {
                            adorner.Pen = adornerPen;
                        }
                        else
                        {
                            var adornerBrush = GetDropTargetAdornerBrush(dropInfo.VisualTarget);
                            if (adornerBrush != null)
                            {
                                adorner.Pen.SetCurrentValue(Pen.BrushProperty, adornerBrush);
                            }
                        }

                        adorner.DropInfo = dropInfo;
                        adorner.InvalidateVisual();
                    }
                }
            }

            // Set the drag effect adorner if there is one
            if (dragInfo != null)
            {
                if (DragDropEffectPreview is null)
                {
                    DragDropEffectPreview = GetDragDropEffectPreview(dropInfo, sender as UIElement);
                    DragDropEffectPreview?.Move(e.GetPosition(DragDropEffectPreview.PlacementTarget));
                }
                else if (DragDropEffectPreview.Effects != dropInfo.Effects || DragDropEffectPreview.EffectText != dropInfo.EffectText || DragDropEffectPreview.DestinationText != dropInfo.DestinationText)
                {
                    DragDropEffectPreview.Effects = dropInfo.Effects;
                    DragDropEffectPreview.EffectText = dropInfo.EffectText;
                    DragDropEffectPreview.DestinationText = dropInfo.DestinationText;

                    var template = GetDragDropEffectTemplate(dragInfo.VisualSource, dropInfo);
                    if (template is null)
                    {
                        DragDropEffectPreview = null;
                    }
                    else
                    {
                        ((ContentPresenter)DragDropEffectPreview.Child).SetCurrentValue(ContentPresenter.ContentTemplateProperty, template);
                    }
                }
            }

            e.Effects = dropInfo.Effects;
            e.Handled = !dropInfo.NotHandled;

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private static void DropTargetOnDrop(object sender, DragEventArgs e)
        {
            DropTargetOnDrop(sender, e, EventType.Bubbled);
        }

        private static void DropTargetOnPreviewDrop(object sender, DragEventArgs e)
        {
            DropTargetOnDrop(sender, e, EventType.Tunneled);
        }

        private static void DropTargetOnDrop(object sender, DragEventArgs e, EventType eventType)
        {
            var dragInfo = _dragInfo;
            var dropInfoBuilder = TryGetDropInfoBuilder(sender as DependencyObject);
            var dropInfo = dropInfoBuilder?.CreateDropInfo(sender, e, dragInfo, eventType) ?? new DropInfo(sender, e, dragInfo, eventType);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
            var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
            var itemsSorter = TryGetDropTargetItemsSorter(dropInfo, sender as UIElement);

            DragDropPreview = null;
            DragDropEffectPreview = null;
            DropTargetAdorner = null;

            dropHandler.DragOver(dropInfo);

            if (itemsSorter != null && dropInfo.Data is IEnumerable enumerable and not string)
            {
                dropInfo.Data = itemsSorter.SortDropTargetItems(enumerable);
            }

            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            e.Effects = dropInfo.Effects;
            e.Handled = !dropInfo.NotHandled;

            Mouse.OverrideCursor = null;
            SetIsDragLeaved(sender as DependencyObject, true);
        }

        private static void DragSourceOnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (DragDropEffectPreview != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
                if (Mouse.OverrideCursor != Cursors.Arrow)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
                if (Mouse.OverrideCursor != null)
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private static void DropTargetOnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (DragDropEffectPreview != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
                if (Mouse.OverrideCursor != Cursors.Arrow)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
                if (Mouse.OverrideCursor != null)
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private static bool GetHitTestResult(object sender, Point elementPosition)
        {
            return ((sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition))
                   || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
                   || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
                   || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
                   || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
                   || HitTestUtilities.HitTest4Type<MenuBase>(sender, elementPosition)
                   || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                   || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition);
        }

        private static DragDropPreview dragDropPreview;

        private static DragDropPreview DragDropPreview
        {
            get => dragDropPreview;
            set
            {
                dragDropPreview?.SetCurrentValue(Popup.IsOpenProperty, false);
                dragDropPreview = value;
            }
        }

        private static DragDropEffectPreview dragDropEffectPreview;

        private static DragDropEffectPreview DragDropEffectPreview
        {
            get => dragDropEffectPreview;
            set
            {
                if (dragDropEffectPreview is { })
                {
                    dragDropEffectPreview.SetCurrentValue(Popup.PopupAnimationProperty, PopupAnimation.None);
                    dragDropEffectPreview.SetCurrentValue(Popup.IsOpenProperty, false);
                }

                dragDropEffectPreview = value;
            }
        }

        private static DropTargetAdorner dropTargetAdorner;

        private static DropTargetAdorner DropTargetAdorner
        {
            get => dropTargetAdorner;
            set
            {
                dropTargetAdorner?.Detatch();
                dropTargetAdorner = value;
            }
        }

        private static DragInfo _dragInfo;
        private static bool _dragInProgress;
        private static object _clickSupressItem;

        internal static readonly DependencyProperty IsDragOverProperty
            = DependencyProperty.RegisterAttached("IsDragOver",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(bool)));

        /// <summary>Helper for setting <see cref="IsDragOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsDragOverProperty"/> on.</param>
        /// <param name="value">IsDragOver property value.</param>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        internal static void SetIsDragOver(DependencyObject element, bool value)
        {
            element.SetValue(IsDragOverProperty, value);
        }

        /// <summary>Helper for getting <see cref="IsDragOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsDragOverProperty"/> from.</param>
        /// <returns>IsDragOver property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        internal static bool GetIsDragOver(DependencyObject element)
        {
            return (bool)element.GetValue(IsDragOverProperty);
        }

        internal static readonly DependencyProperty IsDragLeavedProperty
            = DependencyProperty.RegisterAttached("IsDragLeaved",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(true));

        /// <summary>Helper for setting <see cref="IsDragLeavedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsDragLeavedProperty"/> on.</param>
        /// <param name="value">IsDragLeaved property value.</param>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        internal static void SetIsDragLeaved(DependencyObject element, bool value)
        {
            element.SetValue(IsDragLeavedProperty, value);
        }

        /// <summary>Helper for getting <see cref="IsDragLeavedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsDragLeavedProperty"/> from.</param>
        /// <returns>IsDragLeaved property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        internal static bool GetIsDragLeaved(DependencyObject element)
        {
            return (bool)element.GetValue(IsDragLeavedProperty);
        }
    }
}