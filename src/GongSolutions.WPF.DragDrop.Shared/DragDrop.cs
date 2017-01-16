using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Icons;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Media.Imaging;

namespace GongSolutions.Wpf.DragDrop
{
  public static partial class DragDrop
  {
    private static void CreateDragAdorner(DropInfo dropInfo)
    {
      var template = GetDragAdornerTemplate(m_DragInfo.VisualSource);
      var templateSelector = GetDragAdornerTemplateSelector(m_DragInfo.VisualSource);

      UIElement adornment = null;

      var useDefaultDragAdorner = GetUseDefaultDragAdorner(m_DragInfo.VisualSource);
      var useVisualSourceItemSizeForDragAdorner = GetUseVisualSourceItemSizeForDragAdorner(m_DragInfo.VisualSource);

      if (template == null && templateSelector == null && useDefaultDragAdorner) {
        var bs = CaptureScreen(m_DragInfo.VisualSourceItem, m_DragInfo.VisualSourceFlowDirection);
        if (bs != null) {
          var factory = new FrameworkElementFactory(typeof(Image));
          factory.SetValue(Image.SourceProperty, bs);
          factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
          factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
          factory.SetValue(FrameworkElement.WidthProperty, bs.Width);
          factory.SetValue(FrameworkElement.HeightProperty, bs.Height);
          factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
          factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
          template = new DataTemplate { VisualTree = factory };
        }
      }

      if (template != null || templateSelector != null) {
        if (m_DragInfo.Data is IEnumerable && !(m_DragInfo.Data is string)) {
          if (!useDefaultDragAdorner && ((IEnumerable)m_DragInfo.Data).Cast<object>().Count() <= 10) {
            var itemsControl = new ItemsControl();
            itemsControl.ItemsSource = (IEnumerable)m_DragInfo.Data;
            itemsControl.ItemTemplate = template;
            itemsControl.ItemTemplateSelector = templateSelector;

            if (useVisualSourceItemSizeForDragAdorner)
            {
              var bounds = VisualTreeHelper.GetDescendantBounds(m_DragInfo.VisualSourceItem);
              itemsControl.SetValue(FrameworkElement.MinWidthProperty, bounds.Width);
            }

            // The ItemsControl doesn't display unless we create a grid to contain it.
            // Not quite sure why we need this...
            var grid = new Grid();
            grid.Children.Add(itemsControl);
            adornment = grid;
          }
        } else {
          var contentPresenter = new ContentPresenter();
          contentPresenter.Content = m_DragInfo.Data;
          contentPresenter.ContentTemplate = template;
          contentPresenter.ContentTemplateSelector = templateSelector;

          if (useVisualSourceItemSizeForDragAdorner)
          {
            var bounds = VisualTreeHelper.GetDescendantBounds(m_DragInfo.VisualSourceItem);
            contentPresenter.SetValue(FrameworkElement.MinWidthProperty, bounds.Width);
            contentPresenter.SetValue(FrameworkElement.MinHeightProperty, bounds.Height);
          }

          adornment = contentPresenter;
        }
      }

      if (adornment != null) {
        if (useDefaultDragAdorner) {
          adornment.Opacity = GetDefaultDragAdornerOpacity(m_DragInfo.VisualSource);
        }

        var rootElement = RootElementFinder.FindRoot(dropInfo.VisualTarget ?? m_DragInfo.VisualSource);
        DragAdorner = new DragAdorner(rootElement, adornment);
      }
    }

    // Helper to generate the image - I grabbed this off Google 
    // somewhere. -- Chris Bordeman cbordeman@gmail.com
    private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection)
    {
      if (target == null) {
        return null;
      }

      var dpiX = DpiHelper.DpiX;
      var dpiY = DpiHelper.DpiY;

      var bounds = VisualTreeHelper.GetDescendantBounds(target);
      var dpiBounds = DpiHelper.LogicalRectToDevice(bounds);

      var pixelWidth = (int)Math.Ceiling(dpiBounds.Width);
      var pixelHeight = (int)Math.Ceiling(dpiBounds.Height);
      if (pixelWidth < 0 || pixelHeight < 0)
      {
        return null;
      }

      var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);

      var dv = new DrawingVisual();
      using (var ctx = dv.RenderOpen()) {
        var vb = new VisualBrush(target);
        if (flowDirection == FlowDirection.RightToLeft) {
          var transformGroup = new TransformGroup();
          transformGroup.Children.Add(new ScaleTransform(-1, 1));
          transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width - 1, 0));
          ctx.PushTransform(transformGroup);
        }
        ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
      }

      rtb.Render(dv);
      
      return rtb;
    }

    private static void CreateEffectAdorner(DropInfo dropInfo)
    {
      var template = GetEffectAdornerTemplate(m_DragInfo.VisualSource, dropInfo.Effects, dropInfo.DestinationText);

      if (template != null) {
        var rootElement = RootElementFinder.FindRoot(dropInfo.VisualTarget ?? m_DragInfo.VisualSource);

        var adornment = new ContentPresenter();
        adornment.Content = m_DragInfo.Data;
        adornment.ContentTemplate = template;

        EffectAdorner = new DragAdorner(rootElement, adornment, dropInfo.Effects);
      }
    }

    private static DataTemplate GetEffectAdornerTemplate(UIElement target, DragDropEffects effect, string destinationText)
    {
      switch (effect)
      {
        case DragDropEffects.All:
          // TODO: Add default template for EffectAll
          return GetEffectAllAdornerTemplate(target);
        case DragDropEffects.Copy:
          return GetEffectCopyAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectCopy, "Copy to", destinationText);
        case DragDropEffects.Link:
          return GetEffectLinkAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectLink, "Link to", destinationText);
        case DragDropEffects.Move:
          return GetEffectMoveAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectMove, "Move to", destinationText);
        case DragDropEffects.None:
          return GetEffectNoneAdornerTemplate(target) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectNone, "None", destinationText);
        case DragDropEffects.Scroll:
          // TODO: Add default template EffectScroll
          return GetEffectScrollAdornerTemplate(target);
        default:
          return null;
      }
    }

    private static DataTemplate CreateDefaultEffectDataTemplate(UIElement target, BitmapImage effectIcon, string effectText, string destinationText)
    {
      if (!GetUseDefaultEffectDataTemplate(target)) {
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
      var stopCollection = new GradientStopCollection {
                                                        new GradientStop(Colors.White, 0.0),
                                                        new GradientStop(Colors.AliceBlue, 1.0)
                                                      };
      var gradientBrush = new LinearGradientBrush(stopCollection) {
                                                                    StartPoint = new Point(0, 0),
                                                                    EndPoint = new Point(0, 1)
                                                                  };
      borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
      borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
      borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
      borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
      borderFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);
#if !NET35
      borderFactory.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
      borderFactory.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
      borderFactory.SetValue(TextOptions.TextHintingModeProperty, TextHintingMode.Fixed);
#endif
      borderFactory.AppendChild(stackPanelFactory);

      // Finally add content to template
      return new DataTemplate { VisualTree = borderFactory };
    }

    private static void Scroll(DropInfo dropInfo, DragEventArgs e)
    {
      if (dropInfo == null || dropInfo.TargetScrollViewer == null)
      {
        return;
      }

      var scrollViewer = dropInfo.TargetScrollViewer;
      var scrollingMode = dropInfo.VisualTarget != null ? GetDropScrollingMode(dropInfo.VisualTarget) : ScrollingMode.Both;

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

    /// <summary>
    /// Gets the drag handler from the drag info or from the sender, if the drag info is null
    /// </summary>
    /// <param name="dragInfo">the drag info object</param>
    /// <param name="sender">the sender from an event, e.g. mouse down, mouse move</param>
    /// <returns></returns>
    private static IDragSource TryGetDragHandler(DragInfo dragInfo, UIElement sender)
    {
      IDragSource dragHandler = null;
      if (dragInfo != null && dragInfo.VisualSource != null)
      {
        dragHandler = GetDragHandler(dragInfo.VisualSource);
      }
      if (dragHandler == null && sender != null)
      {
        dragHandler = GetDragHandler(sender);
      }
      return dragHandler ?? DefaultDragHandler;
    }

    /// <summary>
    /// Gets the drop handler from the drop info or from the sender, if the drop info is null
    /// </summary>
    /// <param name="dropInfo">the drop info object</param>
    /// <param name="sender">the sender from an event, e.g. drag over</param>
    /// <returns></returns>
    private static IDropTarget TryGetDropHandler(DropInfo dropInfo, UIElement sender)
    {
      IDropTarget dropHandler = null;
      if (dropInfo != null && dropInfo.VisualTarget != null)
      {
        dropHandler = GetDropHandler(dropInfo.VisualTarget);
      }
      if (dropHandler == null && sender != null)
      {
        dropHandler = GetDropHandler(sender);
      }
      return dropHandler ?? DefaultDropHandler;
    }

    private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      DoMouseButtonDown(sender, e);
    }

    private static void DragSource_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      DoMouseButtonDown(sender, e);
    }

    private static void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
      var elementPosition = e.GetPosition((IInputElement)sender);
      if (e.ClickCount != 1
          || (sender as UIElement).IsDragSourceIgnored()
          || (e.Source as UIElement).IsDragSourceIgnored()
          || (e.OriginalSource as UIElement).IsDragSourceIgnored()
          || (sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
          || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
          || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
          || HitTestUtilities.IsNotPartOfSender(sender, e)) {
        m_DragInfo = null;
        return;
      }

      m_DragInfo = new DragInfo(sender, e);

      if (m_DragInfo.VisualSourceItem == null)
      {
        m_DragInfo = null;
        return;
      }

      var dragHandler = TryGetDragHandler(m_DragInfo, sender as UIElement);
      if (!dragHandler.CanStartDrag(m_DragInfo)) {
        m_DragInfo = null;
        return;
      }

      // If the sender is a list box that allows multiple selections, ensure that clicking on an 
      // already selected item does not change the selection, otherwise dragging multiple items 
      // is made impossible.
      var itemsControl = sender as ItemsControl;
      if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0 && (Keyboard.Modifiers & ModifierKeys.Control) == 0 && m_DragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems()) {
        var selectedItems = itemsControl.GetSelectedItems().OfType<object>().ToList();
        if (selectedItems.Count > 1 && selectedItems.Contains(m_DragInfo.SourceItem)) {
          m_ClickSupressItem = m_DragInfo.SourceItem;
          e.Handled = true;
        }
      }
    }

    private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      DoMouseButtonUp(sender, e);
    }

    private static void DragSource_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      DoMouseButtonUp(sender, e);
    }

    private static void DoMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
      var elementPosition = e.GetPosition((IInputElement)sender);
      if ((sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)) {
        m_DragInfo = null;
        m_ClickSupressItem = null;
        return;
      }

      // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
      // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
      var itemsControl = sender as ItemsControl;
      if (itemsControl != null && m_DragInfo != null && m_ClickSupressItem != null && m_ClickSupressItem == m_DragInfo.SourceItem) {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
          itemsControl.SetItemSelected(m_DragInfo.SourceItem, false);
        } else if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0) {
          itemsControl.SetSelectedItem(m_DragInfo.SourceItem);
        }
      }

      m_DragInfo = null;
      m_ClickSupressItem = null;
    }

    private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (m_DragInfo != null && !m_DragInProgress) {

        // the start from the source
        var dragStart = m_DragInfo.DragStartPosition;

        // do nothing if mouse left/right button is released or the pointer is captured
        if (m_DragInfo.MouseButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released) {
          m_DragInfo = null;
          return;
        }
        if (DragDrop.GetCanDragWithMouseRightButton(m_DragInfo.VisualSource) && m_DragInfo.MouseButton == MouseButton.Right && e.RightButton == MouseButtonState.Released) {
          m_DragInfo = null;
          return;
        }

        // current mouse position
        var position = e.GetPosition((IInputElement)sender);

        // only if the sender is the source control and the mouse point differs from an offset
        if (m_DragInfo.VisualSource == sender
            && (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)) {
          var dragHandler = TryGetDragHandler(m_DragInfo, sender as UIElement);
          if (dragHandler.CanStartDrag(m_DragInfo)) {
            dragHandler.StartDrag(m_DragInfo);

            if (m_DragInfo.Effects != DragDropEffects.None && m_DragInfo.Data != null) {
              var data = m_DragInfo.DataObject;

              if (data == null) {
                data = new DataObject(DataFormat.Name, m_DragInfo.Data);
              } else {
                data.SetData(DataFormat.Name, m_DragInfo.Data);
              }

              try {
                m_DragInProgress = true;
                var result = System.Windows.DragDrop.DoDragDrop(m_DragInfo.VisualSource, data, m_DragInfo.Effects);
                if (result == DragDropEffects.None)
                  dragHandler.DragCancelled();
              }
              catch (Exception ex) {
                if (!dragHandler.TryCatchOccurredException(ex)) {
                  throw;
                }
              }
              finally {
                m_DragInProgress = false;
              }

              m_DragInfo = null;
            }
          }
        }
      }
    }

    private static void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      if (e.Action == DragAction.Cancel || e.EscapePressed) {
        DragAdorner = null;
        EffectAdorner = null;
        DropTargetAdorner = null;
        Mouse.OverrideCursor = null;
      }
    }

    private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
    {
      DropTarget_PreviewDragOver(sender, e);
    }

    private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
    {
      DragAdorner = null;
      EffectAdorner = null;
      DropTargetAdorner = null;
    }

    private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
    {
      var elementPosition = e.GetPosition((IInputElement)sender);

      var dropInfo = new DropInfo(sender, e, m_DragInfo);
      var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
      var itemsControl = dropInfo.VisualTarget;

      dropHandler.DragOver(dropInfo);

      if (DragAdorner == null && m_DragInfo != null) {
        CreateDragAdorner(dropInfo);
      }

      if (DragAdorner != null) {
        var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

        if (tempAdornerPos.X >= 0 && tempAdornerPos.Y >= 0) {
          _adornerPos = tempAdornerPos;
        }

        // Fixed the flickering adorner - Size changes to zero 'randomly'...?
        if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0) {
          _adornerSize = DragAdorner.RenderSize;
        }

        if (m_DragInfo != null) {
          // move the adorner
          var offsetX = _adornerSize.Width * -GetDragMouseAnchorPoint(m_DragInfo.VisualSource).X;
          var offsetY = _adornerSize.Height * -GetDragMouseAnchorPoint(m_DragInfo.VisualSource).Y;
          _adornerPos.Offset(offsetX, offsetY);
          var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
          var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
          if (adornerPosRightX > maxAdornerPosX) {
            _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
          }
          if (_adornerPos.Y < 0) {
            _adornerPos.Y = 0;
          }
        }

        DragAdorner.MousePosition = _adornerPos;
        DragAdorner.InvalidateVisual();
      }

      if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
          || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
          || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition))
      {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        return;
      }

      // If the target is an ItemsControl then update the drop target adorner.
      if (itemsControl != null) {
        // Display the adorner in the control's ItemsPresenter. If there is no 
        // ItemsPresenter provided by the style, try getting hold of a
        // ScrollContentPresenter and using that.
        var adornedElement =
          itemsControl is TabControl
            ? itemsControl.GetVisualDescendent<TabPanel>()
            : (itemsControl.GetVisualDescendent<ScrollContentPresenter>() ?? itemsControl.GetVisualDescendent<ItemsPresenter>() as UIElement ?? itemsControl);

        if (adornedElement != null) {
          if (dropInfo.DropTargetAdorner == null) {
            DropTargetAdorner = null;
          } else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner)) {
            DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement, dropInfo);
          }

          var adorner = DropTargetAdorner;
          if (adorner != null) {
            var adornerBrush = GetDropTargetAdornerBrush(dropInfo.VisualTarget);
            if (adornerBrush != null)
            {
              adorner.Pen.Brush = adornerBrush;
            }
            adorner.DropInfo = dropInfo;
            adorner.InvalidateVisual();
          }
        }
      }

      // Set the drag effect adorner if there is one
      if (m_DragInfo != null && (EffectAdorner == null || EffectAdorner.Effects != dropInfo.Effects)) {
        CreateEffectAdorner(dropInfo);
      }

      if (EffectAdorner != null) {
        var adornerPos = e.GetPosition(EffectAdorner.AdornedElement);
        adornerPos.Offset(20, 20);
        EffectAdorner.MousePosition = adornerPos;
        EffectAdorner.InvalidateVisual();
      }

      e.Effects = dropInfo.Effects;
      e.Handled = !dropInfo.NotHandled;

      if (!dropInfo.IsSameDragDropContextAsSource)
      {
          e.Effects = DragDropEffects.None;
      }

      Scroll(dropInfo, e);
    }

    private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
    {
      var dropInfo = new DropInfo(sender, e, m_DragInfo);
      var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
      var dragHandler = TryGetDragHandler(m_DragInfo, sender as UIElement);

      DragAdorner = null;
      EffectAdorner = null;
      DropTargetAdorner = null;

      dropHandler.DragOver(dropInfo);
      dropHandler.Drop(dropInfo);
      dragHandler.Dropped(dropInfo);

      Mouse.OverrideCursor = null;
      e.Handled = !dropInfo.NotHandled;
    }

    private static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (EffectAdorner != null)
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

    private static DragAdorner _DragAdorner;

    private static DragAdorner DragAdorner
    {
      get { return _DragAdorner; }
      set
      {
        _DragAdorner?.Detatch();
        _DragAdorner = value;
      }
    }

    private static DragAdorner _EffectAdorner;

    private static DragAdorner EffectAdorner
    {
      get { return _EffectAdorner; }
      set
      {
        _EffectAdorner?.Detatch();
        _EffectAdorner = value;
      }
    }

    private static DropTargetAdorner _DropTargetAdorner;

    private static DropTargetAdorner DropTargetAdorner
    {
      get { return _DropTargetAdorner; }
      set
      {
        _DropTargetAdorner?.Detatch();
        _DropTargetAdorner = value;
      }
    }

    private static DragInfo m_DragInfo;
    private static bool m_DragInProgress;
    private static object m_ClickSupressItem;
    private static Point _adornerPos;
    private static Size _adornerSize;
  }
}
