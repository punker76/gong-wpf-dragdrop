using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GongSolutions.Wpf.DragDrop.Icons;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Media.Imaging;

namespace GongSolutions.Wpf.DragDrop
{
    public static class DragDrop
    {
        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);

            if (template == null)
            {
                FrameworkElementFactory imageSourceFactory = new FrameworkElementFactory(typeof(Image));
                imageSourceFactory.SetValue(Image.SourceProperty, IconFactory.EffectNone);
                imageSourceFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
                imageSourceFactory.SetValue(FrameworkElement.WidthProperty, 12.0);

                template = new DataTemplate();
                template.VisualTree = imageSourceFactory;
            }

            return template;
        }

        public static void SetEffectNoneAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectNoneAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty);

            if (template == null)
                template = CreateDefaultEffectDataTemplate(IconFactory.EffectCopy, "Copy to", destinationText);

            return template;
        }

        public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectCopyAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty);

            if (template == null)
                template = CreateDefaultEffectDataTemplate(IconFactory.EffectMove, "Move to", destinationText);

            return template;
        }

        public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectMoveAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectLinkAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectLinkAdornerTemplateProperty);

            if (template == null)
                template = CreateDefaultEffectDataTemplate(IconFactory.EffectLink, "Link to", destinationText);

            return template;
        }

        public static void SetEffectLinkAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectLinkAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectAllAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectAllAdornerTemplateProperty);

            // TODO: Add default template

            return template;
        }

        public static void SetEffectAllAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectAllAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectScrollAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectScrollAdornerTemplateProperty);

            // TODO: Add default template

            return template;
        }

        public static void SetEffectScrollAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectScrollAdornerTemplateProperty, value);
        }

        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        public static IDragSource GetDragHandler(UIElement target)
        {
            return (IDragSource)target.GetValue(DragHandlerProperty);
        }

        public static void SetDragHandler(UIElement target, IDragSource value)
        {
            target.SetValue(DragHandlerProperty, value);
        }

        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        public static bool GetDragSourceIgnore(UIElement target)
        {
            return (bool)target.GetValue(DragSourceIgnoreProperty);
        }

        public static void SetDragSourceIgnore(UIElement target, bool value)
        {
            target.SetValue(DragSourceIgnoreProperty, value);
        }

        public static IDragSource DefaultDragHandler
        {
            get
            {
                if (m_DefaultDragHandler == null)
                {
                    m_DefaultDragHandler = new DefaultDragHandler();
                }

                return m_DefaultDragHandler;
            }
            set { m_DefaultDragHandler = value; }
        }

        public static IDropTarget DefaultDropHandler
        {
            get
            {
                if (m_DefaultDropHandler == null)
                {
                    m_DefaultDropHandler = new DefaultDropHandler();
                }

                return m_DefaultDropHandler;
            }
            set { m_DefaultDropHandler = value; }
        }

        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty EffectLinkAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectLinkAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty EffectAllAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectAllAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty EffectScrollAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("EffectScrollAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDrop));

        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));

        public static readonly DependencyProperty DragSourceIgnoreProperty =
            DependencyProperty.RegisterAttached("DragSourceIgnore", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop),
                                                new UIPropertyMetadata(false, IsDragSourceChanged));

        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop),
                                                new UIPropertyMetadata(false, IsDropTargetChanged));

        public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
                uiElement.QueryContinueDrag += DragSource_QueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
                uiElement.QueryContinueDrag -= DragSource_QueryContinueDrag;
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uiElement.AllowDrop = true;
                //uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
                //uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
                //uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
                //uiElement.PreviewDrop += DropTarget_PreviewDropTest;

                uiElement.DragEnter += DropTarget_PreviewDragEnter;
                uiElement.DragLeave += DropTarget_PreviewDragLeave;
                uiElement.DragOver += DropTarget_PreviewDragOver;
                uiElement.Drop += DropTarget_PreviewDrop;
                uiElement.GiveFeedback += DropTarget_GiveFeedback;
            }
            else
            {
                uiElement.AllowDrop = false;
                //uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                //uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                //uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
                //uiElement.PreviewDrop -= DropTarget_PreviewDropTest;

                uiElement.DragEnter -= DropTarget_PreviewDragEnter;
                uiElement.DragLeave -= DropTarget_PreviewDragLeave;
                uiElement.DragOver -= DropTarget_PreviewDragOver;
                uiElement.Drop -= DropTarget_PreviewDrop;
                uiElement.GiveFeedback -= DropTarget_GiveFeedback;

                Mouse.OverrideCursor = null;
            }
        }

        static void CreateDragAdorner()
        {
            DataTemplate template = GetDragAdornerTemplate(m_DragInfo.VisualSource);

            UIElement rootElement = null;
            Window parentWindow = m_DragInfo.VisualSource.GetVisualAncestor<Window>();
            UIElement adornment = null;

            if (parentWindow != null)
            {
                rootElement = parentWindow.Content as UIElement;
            }
            if (rootElement == null)
            {
                rootElement = (UIElement)Application.Current.MainWindow.Content;
            }

            if (template != null)
            {
                if (m_DragInfo.Data is IEnumerable && !(m_DragInfo.Data is string))
                {
                    if (((IEnumerable)m_DragInfo.Data).Cast<object>().Count() <= 10)
                    {
                        ItemsControl itemsControl = new ItemsControl();
                        itemsControl.ItemsSource = (IEnumerable)m_DragInfo.Data;
                        itemsControl.ItemTemplate = template;

                        // The ItemsControl doesn't display unless we create a border to contain it.
                        // Not quite sure why this is...
                        Border border = new Border();
                        border.Child = itemsControl;
                        adornment = border;
                    }
                }
                else
                {
                    ContentPresenter contentPresenter = new ContentPresenter();
                    contentPresenter.Content = m_DragInfo.Data;
                    contentPresenter.ContentTemplate = template;
                    adornment = contentPresenter;
                }
            }
            else
            {
                // Create a default adornor of the item you're dragging if there's isn't a custom one set
//                RenderTargetBitmap bitmap = new RenderTargetBitmap(
//                    Convert.ToInt32(m_DragInfo.VisualSourceItem.DesiredSize.Width),
//                    Convert.ToInt32(m_DragInfo.VisualSourceItem.DesiredSize.Height),
//                    96, 96, PixelFormats.Pbgra32);
//
//                bitmap.Render(m_DragInfo.VisualSourceItem);
//                bitmap.Freeze();

                // Create a default adornor of the item you're dragging if there's isn't a custom one set
                Rectangle rect = new Rectangle();
                var visualBrush = new VisualBrush(m_DragInfo.VisualSourceItem);
                rect.Fill = visualBrush.CloneCurrentValue();
                rect.Width = m_DragInfo.VisualSourceItem.DesiredSize.Width;
                rect.Height = m_DragInfo.VisualSourceItem.DesiredSize.Height;
                rect.IsHitTestVisible = false;
                adornment = rect;
            }

            if (adornment != null)
            {
                adornment.Opacity = 0.5;
                DragAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        static void CreateEffectAdorner(DropInfo dropInfo)
        {
            DataTemplate template = GetEffectAdornerTemplate(m_DragInfo.VisualSource, dropInfo.Effects, dropInfo.DestinationText);

            if (template != null)
            {
                UIElement rootElement = (UIElement)Application.Current.MainWindow.Content;
                UIElement adornment = null;

                ContentPresenter contentPresenter = new ContentPresenter();
                contentPresenter.Content = m_DragInfo.Data;
                contentPresenter.ContentTemplate = template;

                adornment = contentPresenter;

                EffectAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(BitmapImage effectIcon, string effectText)
        {
            return CreateDefaultEffectDataTemplate(effectIcon, effectText, string.Empty);
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(BitmapImage effectIcon, string effectText, string destinationText)
        {
            // Add icon
            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetValue(Image.SourceProperty, effectIcon);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.WidthProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 3.0, 0.0));

            // Add effect text
            var effectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            effectTextBlockFactory.SetValue(TextBlock.TextProperty, effectText);
            effectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            effectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);

            // Add destination text
            var destinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            destinationTextBlockFactory.SetValue(TextBlock.TextProperty, destinationText);
            destinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            destinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            destinationTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3, 0, 0, 0));
            destinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);

            // Create containing panel
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2.0));
            stackPanelFactory.AppendChild(imageFactory);
            stackPanelFactory.AppendChild(effectTextBlockFactory);
            stackPanelFactory.AppendChild(destinationTextBlockFactory);

            // Add border
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            var stopCollection = new GradientStopCollection { new GradientStop(Colors.White, 0.0), 
                                                              new GradientStop(Colors.AliceBlue, 1.0)};
            var gradientBrush = new LinearGradientBrush(stopCollection)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            borderFactory.AppendChild(stackPanelFactory);

            // Finally add content to template
            DataTemplate template = new DataTemplate();
            template.VisualTree = borderFactory;
            return template;
        }

        static DataTemplate GetEffectAdornerTemplate(UIElement target, DragDropEffects effect, string destinationText)
        {
            switch (effect)
            {
                case DragDropEffects.All:
                    return null;
                case DragDropEffects.Copy:
                    return GetEffectCopyAdornerTemplate(target, destinationText);
                case DragDropEffects.Link:
                    return GetEffectLinkAdornerTemplate(target, destinationText);
                case DragDropEffects.Move:
                    return GetEffectMoveAdornerTemplate(target, destinationText);
                case DragDropEffects.None:
                    return GetEffectNoneAdornerTemplate(target);
                case DragDropEffects.Scroll:
                    return null;
                default:
                    return null;
            }
        }

        private static bool HitTestScrollBar(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            if (hit == null)
            {
                return false;
            }
            else
            {
                var scrollBar = hit.VisualHit.GetVisualAncestor<System.Windows.Controls.Primitives.ScrollBar>();
                return scrollBar != null && scrollBar.Visibility == Visibility.Visible;
            }
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var scrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer != null)
            {
                var position = e.GetPosition(scrollViewer);
                var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                         scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            if (e.ClickCount != 1 || HitTestScrollBar(sender, e) || GetDragSourceIgnore((UIElement)sender))
            {
                m_DragInfo = null;
                return;
            }

            m_DragInfo = new DragInfo(sender, e);

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            var itemsControl = sender as ItemsControl;

            if (m_DragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().Cast<object>();

                if (selectedItems.Count() > 1 && selectedItems.Contains(m_DragInfo.SourceItem))
                {
                    m_ClickSupressItem = m_DragInfo.SourceItem;
                    e.Handled = true;
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            var itemsControl = sender as ItemsControl;

            if (itemsControl != null && m_DragInfo != null && m_ClickSupressItem == m_DragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    itemsControl.SetItemSelected(m_DragInfo.SourceItem, false);
                }
                else
                {
                    itemsControl.SetSelectedItem(m_DragInfo.SourceItem);
                }
            }

            if (m_DragInfo != null)
            {
                m_DragInfo = null;
            }

            m_ClickSupressItem = null;
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_DragInfo != null && !m_DragInProgress)
            {
                var dragStart = m_DragInfo.DragStartPosition;
                var position = e.GetPosition((IInputElement)sender);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragHandler = GetDragHandler(m_DragInfo.VisualSource);

                    if (dragHandler != null)
                    {
                        dragHandler.StartDrag(m_DragInfo);
                    }
                    else
                    {
                        DefaultDragHandler.StartDrag(m_DragInfo);
                    }

                    if (m_DragInfo  .Effects != DragDropEffects.None && m_DragInfo.Data != null)
                    {
                        var data = m_DragInfo.DataObject;

                        if (data == null)
                        {
                            data = new DataObject(DataFormat.Name, m_DragInfo.Data);
                        }
                        else
                        {
                            data.SetData(DataFormat.Name, m_DragInfo.Data);
                        }

                        try
                        {
                            m_DragInProgress = true;
                            System.Windows.DragDrop.DoDragDrop(m_DragInfo.VisualSource, data, m_DragInfo.Effects);
                        }
                        finally
                        {
                            m_DragInProgress = false;
                        }

                        m_DragInfo = null;
                    }
                }
            }
        }

        static void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed)
            {
                DragAdorner = null;
                EffectAdorner = null;
                DropTargetAdorner = null;
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            Mouse.OverrideCursor = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, m_DragInfo);
            var dropHandler = GetDropHandler((UIElement)sender);
            var itemsControl = sender as ItemsControl;

            if (dropHandler != null)
            {
                dropHandler.DragOver(dropInfo);
            }
            else
            {
                DefaultDropHandler.DragOver(dropInfo);
            }

            if (DragAdorner == null && m_DragInfo != null)
            {
                CreateDragAdorner();
            }

            if (DragAdorner != null) {
                var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (tempAdornerPos.X > 0 && tempAdornerPos.Y > 0)
                    _adornerPos = tempAdornerPos;

                // Fixed the flickering adorner - Size changes to zero 'randomly'...?
                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                    _adornerSize = DragAdorner.RenderSize;

                if (m_DragInfo != null)
                {
                    // When there is a custom adorner move to above the cursor and center it
                    if (GetDragAdornerTemplate(m_DragInfo.VisualSource) != null)
                    {
                        _adornerPos.Offset((_adornerSize.Width * -0.5), (_adornerSize.Height * -0.9));
                    }
                    else
                    {
                        _adornerPos.Offset(m_DragInfo.PositionInDraggedItem.X * -1, m_DragInfo.PositionInDraggedItem.Y * -1);
                    }
                }

                DragAdorner.MousePosition = _adornerPos;
                DragAdorner.InvalidateVisual();
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (itemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                var adornedElement =
                    (UIElement)itemsControl.GetVisualDescendent<ItemsPresenter>() ??
                    (UIElement)itemsControl.GetVisualDescendent<ScrollContentPresenter>();

                if (adornedElement != null)
                {
                    if (dropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);
                    }

                    if (DropTargetAdorner != null)
                    {
                        DropTargetAdorner.DropInfo = dropInfo;
                        DropTargetAdorner.InvalidateVisual();
                    }
                }
            }

            // Set the drag effect adorner if there is one
            if (EffectAdorner == null && m_DragInfo != null) {
                CreateEffectAdorner(dropInfo);
            }

            if (EffectAdorner != null) {
                var adornerPos = e.GetPosition(EffectAdorner.AdornedElement);
                adornerPos.Offset(20, 20);
                EffectAdorner.MousePosition = adornerPos;
                EffectAdorner.InvalidateVisual();
            }

            e.Effects = dropInfo.Effects;
            e.Handled = true;

            Scroll((DependencyObject)sender, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, m_DragInfo);
            var dropHandler = GetDropHandler((UIElement)sender) ?? DefaultDropHandler;
            var dragHandler = GetDragHandler((UIElement)sender) ?? DefaultDragHandler;

            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = true;
        }

        static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        private static DragAdorner DragAdorner
        {
            get { return m_DragAdorner; }
            set
            {
                if (m_DragAdorner != null)
                {
                    m_DragAdorner.Detatch();
                }

                m_DragAdorner = value;
            }
        }

        static DragAdorner EffectAdorner
        {
            get { return m_EffectAdorner; }
            set
            {
                if (m_EffectAdorner != null)
                {
                    m_EffectAdorner.Detatch();
                }

                m_EffectAdorner = value;
            }
        }

        private static DropTargetAdorner DropTargetAdorner
        {
            get { return m_DropTargetAdorner; }
            set
            {
                if (m_DropTargetAdorner != null)
                {
                    m_DropTargetAdorner.Detatch();
                }

                m_DropTargetAdorner = value;
            }
        }

        private static IDragSource m_DefaultDragHandler;
        private static IDropTarget m_DefaultDropHandler;
        private static DragAdorner m_DragAdorner;
        static DragAdorner m_EffectAdorner;
        private static DragInfo m_DragInfo;
        private static bool m_DragInProgress;
        private static DropTargetAdorner m_DropTargetAdorner;
        private static object m_ClickSupressItem;
        private static Point _adornerPos;
        private static Size _adornerSize;
    }
}