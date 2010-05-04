using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Utilities;

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
            set
            {
                m_DefaultDragHandler = value;
            }
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
            set
            {
                m_DefaultDropHandler = value;
            }
        }

        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDrop));

        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop),
                new UIPropertyMetadata(false, IsDragSourceChanged));

        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

        static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
            }
        }

        static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = (UIElement)d;

            if ((bool)e.NewValue == true)
            {
                uiElement.AllowDrop = true;
                uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
                uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
                uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
                uiElement.PreviewDrop += DropTarget_PreviewDrop;
            }
            else
            {
                uiElement.AllowDrop = false;
                uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
                uiElement.PreviewDrop -= DropTarget_PreviewDrop;
            }
        }

        static void CreateDragAdorner()
        {
            DataTemplate template = GetDragAdornerTemplate(m_DragInfo.VisualSource);

            if (template != null)
            {
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

                if (adornment != null)
                {
                    adornment.Opacity = 0.5;
                    DragAdorner = new DragAdorner(rootElement, adornment);
                }
            }
        }

        static bool HitTestScrollBar(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            return hit.VisualHit.GetVisualAncestor<System.Windows.Controls.Primitives.ScrollBar>() != null;
        }

        static void Scroll(DependencyObject o, DragEventArgs e)
        {
            ScrollViewer scrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer != null)
            {
                Point position = e.GetPosition(scrollViewer);
                double scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

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

        static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if the user has clicked on a scrollbar.
            if (HitTestScrollBar(sender, e))
            {
                m_DragInfo = null;
                return;
            }

            m_DragInfo = new DragInfo(sender, e);

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            ItemsControl itemsControl = sender as ItemsControl;

            if (m_DragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                IEnumerable<object> selectedItems = itemsControl.GetSelectedItems().Cast<object>();

                if (selectedItems.Count() > 1 && selectedItems.Contains(m_DragInfo.SourceItem))
                {
                    // TODO: Re-raise the supressed event if the user didn't initiate a drag.
                    e.Handled = true;
                }
            }
        }

        static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If the sender is allows multiple selections, select the DragInfo source item
            ItemsControl itemsControl = sender as ItemsControl;

            if (m_DragInfo != null && 
                m_DragInfo.VisualSourceItem != null && 
                itemsControl != null && 
                itemsControl.CanSelectMultipleItems() && 
                Keyboard.Modifiers != ModifierKeys.Shift && 
                Keyboard.Modifiers != ModifierKeys.Control &&
                Keyboard.Modifiers != (ModifierKeys.Control | ModifierKeys.Shift))
            {
                IEnumerable<object> selectedItems = itemsControl.GetSelectedItems().Cast<object>();

                if (selectedItems.Count() > 1 && selectedItems.Contains(m_DragInfo.SourceItem))
                {
                    itemsControl.SetSelectedItem(m_DragInfo.SourceItem);
                }
            }

            if (m_DragInfo != null)
            {
                m_DragInfo = null;
            }
        }

        static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_DragInfo != null && !m_DragInProgress)
            {
                Point dragStart = m_DragInfo.DragStartPosition;
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    IDragSource dragHandler = GetDragHandler(m_DragInfo.VisualSource);

                    if (dragHandler != null)
                    {
                        dragHandler.StartDrag(m_DragInfo);
                    }
                    else
                    {
                        DefaultDragHandler.StartDrag(m_DragInfo);
                    }

                    if (m_DragInfo.Effects != DragDropEffects.None && m_DragInfo.Data != null)
                    {
                        DataObject data = new DataObject(DataFormat.Name, m_DragInfo.Data);

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

        static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);
        }

        static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            DropTargetAdorner = null;
        }

        static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            DropInfo dropInfo = new DropInfo(sender, e, m_DragInfo);
            IDropTarget dropHandler = GetDropHandler((UIElement)sender);
            ItemsControl itemsControl = sender as ItemsControl;

            if (dropHandler != null)
            {
                dropHandler.DragOver(dropInfo);
            }
            else
            {
                DefaultDropHandler.DragOver(dropInfo);
            }

            // Update the drag adorner.
            if (dropInfo.Effects != DragDropEffects.None)
            {
                if (DragAdorner == null && m_DragInfo != null)
                {
                    CreateDragAdorner();
                }

                if (DragAdorner != null)
                {
                    DragAdorner.MousePosition = e.GetPosition(DragAdorner.AdornedElement);
                    DragAdorner.InvalidateVisual();
                }
            }
            else
            {
                DragAdorner = null;
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (itemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                UIElement adornedElement = 
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

            e.Effects = dropInfo.Effects;
            e.Handled = true;

            Scroll((DependencyObject)sender, e);
        }

        static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            DropInfo dropInfo = new DropInfo(sender, e, m_DragInfo);
            IDropTarget dropHandler = GetDropHandler((UIElement)sender);

            DragAdorner = null;
            DropTargetAdorner = null;

            if (dropHandler != null)
            {
                dropHandler.Drop(dropInfo);
            }
            else
            {
                DefaultDropHandler.Drop(dropInfo);
            }

            e.Handled = true;
        }

        static DragAdorner DragAdorner
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

        static DropTargetAdorner DropTargetAdorner
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

        static IDragSource m_DefaultDragHandler;
        static IDropTarget m_DefaultDropHandler;
        static DragAdorner m_DragAdorner;
        static DragInfo m_DragInfo;
        static bool m_DragInProgress;
        static DropTargetAdorner m_DropTargetAdorner;
    }
}
