using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GongSolutions.Wpf.DragDrop.Icons;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Media.Imaging;

namespace GongSolutions.Wpf.DragDrop
{
  public static class DragDrop
  {
    public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

    public static readonly DependencyProperty DragAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

    public static DataTemplate GetDragAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
    }

    public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(DragAdornerTemplateProperty, value);
    }

    public static readonly DependencyProperty DragAdornerTemplateSelectorProperty = DependencyProperty.RegisterAttached(
      "DragAdornerTemplateSelector", typeof (DataTemplateSelector), typeof (DragDrop), new PropertyMetadata(default(DataTemplateSelector)));

    public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value) 
    {
      element.SetValue(DragAdornerTemplateSelectorProperty, value);
    }

    public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element) 
    {
      return (DataTemplateSelector) element.GetValue(DragAdornerTemplateSelectorProperty);
    }

    public static readonly DependencyProperty UseDefaultDragAdornerProperty =
      DependencyProperty.RegisterAttached("UseDefaultDragAdorner", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

    public static bool GetUseDefaultDragAdorner(UIElement target)
    {
      return (bool)target.GetValue(UseDefaultDragAdornerProperty);
    }

    public static void SetUseDefaultDragAdorner(UIElement target, bool value)
    {
      target.SetValue(UseDefaultDragAdornerProperty, value);
    }

    public static readonly DependencyProperty DefaultDragAdornerOpacityProperty =
      DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity", typeof(double), typeof(DragDrop), new PropertyMetadata(0.8));

    public static double GetDefaultDragAdornerOpacity(UIElement target)
    {
      return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
    }

    public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
    {
      target.SetValue(DefaultDragAdornerOpacityProperty, value);
    }

    public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty =
      DependencyProperty.RegisterAttached("UseDefaultEffectDataTemplate", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

    public static bool GetUseDefaultEffectDataTemplate(UIElement target)
    {
      return (bool)target.GetValue(UseDefaultEffectDataTemplateProperty);
    }

    public static void SetUseDefaultEffectDataTemplate(UIElement target, bool value)
    {
      target.SetValue(UseDefaultEffectDataTemplateProperty, value);
    }

    public static readonly DependencyProperty EffectNoneAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

    public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
    {
      var template = (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);

      if (template == null) {
        if (!GetUseDefaultEffectDataTemplate(target)) {
          return null;
        }

        var imageSourceFactory = new FrameworkElementFactory(typeof(Image));
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

    public static readonly DependencyProperty EffectCopyAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

    public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target, string destinationText)
    {
      var template = (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty);

      if (template == null) {
        template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectCopy, "Copy to", destinationText);
      }

      return template;
    }

    public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectCopyAdornerTemplateProperty, value);
    }

    public static readonly DependencyProperty EffectMoveAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

    public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target, string destinationText)
    {
      var template = (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty);

      if (template == null) {
        template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectMove, "Move to", destinationText);
      }

      return template;
    }

    public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectMoveAdornerTemplateProperty, value);
    }

    public static readonly DependencyProperty EffectLinkAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectLinkAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

    public static DataTemplate GetEffectLinkAdornerTemplate(UIElement target, string destinationText)
    {
      var template = (DataTemplate)target.GetValue(EffectLinkAdornerTemplateProperty);

      if (template == null) {
        template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectLink, "Link to", destinationText);
      }

      return template;
    }

    public static void SetEffectLinkAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectLinkAdornerTemplateProperty, value);
    }

    public static readonly DependencyProperty EffectAllAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectAllAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

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

    public static readonly DependencyProperty EffectScrollAdornerTemplateProperty =
      DependencyProperty.RegisterAttached("EffectScrollAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));

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

    public static readonly DependencyProperty IsDragSourceProperty =
      DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDragSourceChanged));

    public static bool GetIsDragSource(UIElement target)
    {
      return (bool)target.GetValue(IsDragSourceProperty);
    }

    public static void SetIsDragSource(UIElement target, bool value)
    {
      target.SetValue(IsDragSourceProperty, value);
    }

    public static readonly DependencyProperty IsDropTargetProperty =
      DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDropTargetChanged));

    public static bool GetIsDropTarget(UIElement target)
    {
      return (bool)target.GetValue(IsDropTargetProperty);
    }

    public static void SetIsDropTarget(UIElement target, bool value)
    {
      target.SetValue(IsDropTargetProperty, value);
    }

    public static readonly DependencyProperty DragHandlerProperty =
      DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDrop));

    public static IDragSource GetDragHandler(UIElement target)
    {
      return (IDragSource)target.GetValue(DragHandlerProperty);
    }

    public static void SetDragHandler(UIElement target, IDragSource value)
    {
      target.SetValue(DragHandlerProperty, value);
    }

    public static readonly DependencyProperty DropHandlerProperty =
      DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));

    public static IDropTarget GetDropHandler(UIElement target)
    {
      return (IDropTarget)target.GetValue(DropHandlerProperty);
    }

    public static void SetDropHandler(UIElement target, IDropTarget value)
    {
      target.SetValue(DropHandlerProperty, value);
    }

    public static readonly DependencyProperty DragSourceIgnoreProperty =
      DependencyProperty.RegisterAttached("DragSourceIgnore", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));

    public static bool GetDragSourceIgnore(UIElement target)
    {
      return (bool)target.GetValue(DragSourceIgnoreProperty);
    }

    public static void SetDragSourceIgnore(UIElement target, bool value)
    {
      target.SetValue(DragSourceIgnoreProperty, value);
    }

    /// <summary>
    /// DragMouseAnchorPoint defines the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
    /// </summary>
    public static readonly DependencyProperty DragMouseAnchorPointProperty =
      DependencyProperty.RegisterAttached("DragMouseAnchorPoint", typeof(Point), typeof(DragDrop), new PropertyMetadata(new Point(0, 1)));

    public static Point GetDragMouseAnchorPoint(UIElement target)
    {
      return (Point)target.GetValue(DragMouseAnchorPointProperty);
    }

    public static void SetDragMouseAnchorPoint(UIElement target, Point value)
    {
      target.SetValue(DragMouseAnchorPointProperty, value);
    }

    public static IDragSource DefaultDragHandler
    {
      get
      {
        if (m_DefaultDragHandler == null) {
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
        if (m_DefaultDropHandler == null) {
          m_DefaultDropHandler = new DefaultDropHandler();
        }

        return m_DefaultDropHandler;
      }
      set { m_DefaultDropHandler = value; }
    }

    private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = (UIElement)d;

      if ((bool)e.NewValue == true) {
        uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
        uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
        uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
        uiElement.QueryContinueDrag += DragSource_QueryContinueDrag;
      } else {
        uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
        uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
        uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
        uiElement.QueryContinueDrag -= DragSource_QueryContinueDrag;
      }
    }

    private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = (UIElement)d;

      if ((bool)e.NewValue == true) {
        uiElement.AllowDrop = true;

        if (uiElement is ItemsControl) {
          // use normal events for ItemsControls
          uiElement.DragEnter += DropTarget_PreviewDragEnter;
          uiElement.DragLeave += DropTarget_PreviewDragLeave;
          uiElement.DragOver += DropTarget_PreviewDragOver;
          uiElement.Drop += DropTarget_PreviewDrop;
          uiElement.GiveFeedback += DropTarget_GiveFeedback;
        } else {
          // issue #85: try using preview events for all other elements than ItemsControls
          uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
          uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
          uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
          uiElement.PreviewDrop += DropTarget_PreviewDrop;
          uiElement.PreviewGiveFeedback += DropTarget_GiveFeedback;
        }
      } else {
        uiElement.AllowDrop = false;

        if (uiElement is ItemsControl) {
          uiElement.DragEnter -= DropTarget_PreviewDragEnter;
          uiElement.DragLeave -= DropTarget_PreviewDragLeave;
          uiElement.DragOver -= DropTarget_PreviewDragOver;
          uiElement.Drop -= DropTarget_PreviewDrop;
          uiElement.GiveFeedback -= DropTarget_GiveFeedback;
        } else {
          uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
          uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
          uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
          uiElement.PreviewDrop -= DropTarget_PreviewDrop;
          uiElement.PreviewGiveFeedback -= DropTarget_GiveFeedback;
        }

        Mouse.OverrideCursor = null;
      }
    }

    private static void CreateDragAdorner()
    {
      var template = GetDragAdornerTemplate(m_DragInfo.VisualSource);
      var templateSelector = GetDragAdornerTemplateSelector(m_DragInfo.VisualSource);

      UIElement adornment = null;

      var useDefaultDragAdorner = GetUseDefaultDragAdorner(m_DragInfo.VisualSource);

      if (template == null && templateSelector == null && useDefaultDragAdorner) {
        template = new DataTemplate();

        var factory = new FrameworkElementFactory(typeof(Image));

        var bs = CaptureScreen(m_DragInfo.VisualSourceItem, m_DragInfo.VisualSourceFlowDirection);
        factory.SetValue(Image.SourceProperty, bs);
        factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
        factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
        if (m_DragInfo.VisualSourceItem is FrameworkElement) {
          factory.SetValue(FrameworkElement.WidthProperty, ((FrameworkElement)m_DragInfo.VisualSourceItem).ActualWidth);
          factory.SetValue(FrameworkElement.HeightProperty, ((FrameworkElement)m_DragInfo.VisualSourceItem).ActualHeight);
          factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
          factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
        }
        template.VisualTree = factory;
      }

      if (template != null || templateSelector != null) {
        if (m_DragInfo.Data is IEnumerable && !(m_DragInfo.Data is string)) {
          if (((IEnumerable)m_DragInfo.Data).Cast<object>().Count() <= 10) {
            var itemsControl = new ItemsControl();
            itemsControl.ItemsSource = (IEnumerable)m_DragInfo.Data;
            itemsControl.ItemTemplate = template;
            itemsControl.ItemTemplateSelector = templateSelector;

            // The ItemsControl doesn't display unless we create a border to contain it.
            // Not quite sure why this is...
            var border = new Border();
            border.Child = itemsControl;
            adornment = border;
          }
        } else {
          var contentPresenter = new ContentPresenter();
          contentPresenter.Content = m_DragInfo.Data;
          contentPresenter.ContentTemplate = template;
          contentPresenter.ContentTemplateSelector = templateSelector;
          adornment = contentPresenter;
        }
      }

      if (adornment != null) {
        if (useDefaultDragAdorner) {
          adornment.Opacity = GetDefaultDragAdornerOpacity(m_DragInfo.VisualSource);
        }

        var parentWindow = m_DragInfo.VisualSource.GetVisualAncestor<Window>();
        var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
        if (rootElement == null && Application.Current != null && Application.Current.MainWindow != null) {
          rootElement = (UIElement)Application.Current.MainWindow.Content;
        }
        //                i don't want the fu... windows forms reference
        //                if (rootElement == null) {
        //                    var elementHost = m_DragInfo.VisualSource.GetVisualAncestor<ElementHost>();
        //                    rootElement = elementHost != null ? elementHost.Child : null;
        //                }
        if (rootElement == null) {
          rootElement = m_DragInfo.VisualSource.GetVisualAncestor<UserControl>();
        }

        DragAdorner = new DragAdorner(rootElement, adornment);
      }
    }

    // Helper to generate the image - I grabbed this off Google 
    // somewhere. -- Chris Bordeman cbordeman@gmail.com
    private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection, double dpiX = 96.0, double dpiY = 96.0)
    {
      if (target == null) {
        return null;
      }

      var bounds = VisualTreeHelper.GetDescendantBounds(target);

      var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                       (int)(bounds.Height * dpiY / 96.0),
                                       dpiX,
                                       dpiY,
                                       PixelFormats.Pbgra32);

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
        var rootElement = (UIElement)Application.Current.MainWindow.Content;
        UIElement adornment = null;

        var contentPresenter = new ContentPresenter();
        contentPresenter.Content = m_DragInfo.Data;
        contentPresenter.ContentTemplate = template;

        adornment = contentPresenter;

        EffectAdorner = new DragAdorner(rootElement, adornment);
      }
    }

    private static DataTemplate CreateDefaultEffectDataTemplate(UIElement target, BitmapImage effectIcon, string effectText, string destinationText)
    {
      if (!GetUseDefaultEffectDataTemplate(target)) {
        return null;
      }

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
      borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
      borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
      borderFactory.AppendChild(stackPanelFactory);

      // Finally add content to template
      var template = new DataTemplate();
      template.VisualTree = borderFactory;
      return template;
    }

    private static DataTemplate GetEffectAdornerTemplate(UIElement target, DragDropEffects effect, string destinationText)
    {
      switch (effect) {
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

    private static void Scroll(DependencyObject o, DragEventArgs e)
    {
      var scrollViewer = o.GetVisualDescendent<ScrollViewer>();

      if (scrollViewer != null) {
        var position = e.GetPosition(scrollViewer);
        var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

        if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
            scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth) {
          scrollViewer.LineRight();
        } else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0) {
          scrollViewer.LineLeft();
        } else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                   scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight) {
          scrollViewer.LineDown();
        } else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0) {
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
      // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
      var elementPosition = e.GetPosition((IInputElement)sender);
      if (e.ClickCount != 1
          || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
          || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
          || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
          || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
          || HitTestUtilities.IsNotPartOfSender(sender, e)
          || GetDragSourceIgnore((UIElement)sender)) {
        m_DragInfo = null;
        return;
      }

      m_DragInfo = new DragInfo(sender, e);

      var dragHandler = TryGetDragHandler(m_DragInfo, sender as UIElement);
      if (!dragHandler.CanStartDrag(m_DragInfo)) {
        m_DragInfo = null;
        return;
      }

      // If the sender is a list box that allows multiple selections, ensure that clicking on an 
      // already selected item does not change the selection, otherwise dragging multiple items 
      // is made impossible.
      var itemsControl = sender as ItemsControl;

      if (m_DragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems()) {
        var selectedItems = itemsControl.GetSelectedItems().Cast<object>();

        if (selectedItems.Count() > 1 && selectedItems.Contains(m_DragInfo.SourceItem)) {
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

      if (itemsControl != null && m_DragInfo != null && m_ClickSupressItem == m_DragInfo.SourceItem) {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
          itemsControl.SetItemSelected(m_DragInfo.SourceItem, false);
        } else {
          itemsControl.SetSelectedItem(m_DragInfo.SourceItem);
        }
      }

      if (m_DragInfo != null) {
        m_DragInfo = null;
      }

      m_ClickSupressItem = null;
    }

    private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (m_DragInfo != null && !m_DragInProgress) {

        // do nothing if mouse left button is released
        if (e.LeftButton == MouseButtonState.Released)
        {
          m_DragInfo = null;
          return;
        } 

        var dragStart = m_DragInfo.DragStartPosition;
        var position = e.GetPosition((IInputElement)sender);

        if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance) {
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
      var elementPosition = e.GetPosition((IInputElement)sender);
      if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
          || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
          || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition)) {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        return;
      }

      var dropInfo = new DropInfo(sender, e, m_DragInfo);
      var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
      var itemsControl = dropInfo.VisualTarget;

      dropHandler.DragOver(dropInfo);

      if (DragAdorner == null && m_DragInfo != null) {
        CreateDragAdorner();
      }

      if (DragAdorner != null) {
        var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

        if (tempAdornerPos.X > 0 && tempAdornerPos.Y > 0) {
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
        }

        DragAdorner.MousePosition = _adornerPos;
        DragAdorner.InvalidateVisual();
      }

      // If the target is an ItemsControl then update the drop target adorner.
      if (itemsControl != null) {
        // Display the adorner in the control's ItemsPresenter. If there is no 
        // ItemsPresenter provided by the style, try getting hold of a
        // ScrollContentPresenter and using that.
        var adornedElement =
          (UIElement)itemsControl.GetVisualDescendent<ItemsPresenter>() ??
          (UIElement)itemsControl.GetVisualDescendent<ScrollContentPresenter>();

        if (adornedElement != null) {
          if (dropInfo.DropTargetAdorner == null) {
            DropTargetAdorner = null;
          } else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner)) {
            DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);
          }

          if (DropTargetAdorner != null) {
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
      e.Handled = !dropInfo.NotHandled;

      Scroll((DependencyObject)dropInfo.VisualTarget, e);
    }

    private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
    {
      var dropInfo = new DropInfo(sender, e, m_DragInfo);
      var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
      var dragHandler = TryGetDragHandler(m_DragInfo, sender as UIElement);

      DragAdorner = null;
      EffectAdorner = null;
      DropTargetAdorner = null;

      dropHandler.Drop(dropInfo);
      dragHandler.Dropped(dropInfo);

      Mouse.OverrideCursor = null;
      e.Handled = !dropInfo.NotHandled;
    }

    private static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (EffectAdorner != null) {
        e.UseDefaultCursors = false;
        e.Handled = true;
      } else {
        e.UseDefaultCursors = true;
        e.Handled = true;
      }
    }

    private static DragAdorner DragAdorner
    {
      get { return m_DragAdorner; }
      set
      {
        if (m_DragAdorner != null) {
          m_DragAdorner.Detatch();
        }

        m_DragAdorner = value;
      }
    }

    private static DragAdorner EffectAdorner
    {
      get { return m_EffectAdorner; }
      set
      {
        if (m_EffectAdorner != null) {
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
        if (m_DropTargetAdorner != null) {
          m_DropTargetAdorner.Detatch();
        }

        m_DropTargetAdorner = value;
      }
    }

    private static IDragSource m_DefaultDragHandler;
    private static IDropTarget m_DefaultDropHandler;
    private static DragAdorner m_DragAdorner;
    private static DragAdorner m_EffectAdorner;
    private static DragInfo m_DragInfo;
    private static bool m_DragInProgress;
    private static DropTargetAdorner m_DropTargetAdorner;
    private static object m_ClickSupressItem;
    private static Point _adornerPos;
    private static Size _adornerSize;
  }
}