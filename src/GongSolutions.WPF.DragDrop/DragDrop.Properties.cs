using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
    public static partial class DragDrop
    {
        /// <summary>
        /// The default data format which will be used for the drag and drop actions.
        /// </summary>
        public static DataFormat DataFormat { get; } = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

        /// <summary>
        /// Gets the default DragHandler.
        /// </summary>
        public static IDragSource DefaultDragHandler { get; } = new DefaultDragHandler();

        /// <summary>
        /// Gets the default DropHandler.
        /// </summary>
        public static IDropTarget DefaultDropHandler { get; } = new DefaultDropHandler();

        /// <summary>
        /// Gets the default RootElementFinder.
        /// </summary>
        public static IRootElementFinder DefaultRootElementFinder { get; } = new RootElementFinder();

        /// <summary>
        /// Gets or sets the data format which will be used for the drag and drop operations.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("WpfAnalyzers.Correctness", "WPF0150:Use nameof() instead of literal.", Justification = "<Pending>")]
        public static readonly DependencyProperty DataFormatProperty
            = DependencyProperty.RegisterAttached("DataFormat",
                                                  typeof(DataFormat),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(DragDrop.DataFormat));

        /// <summary>Helper for getting <see cref="DataFormatProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DataFormatProperty"/> from.</param>
        /// <remarks>Gets the data format which will be used for the drag and drop operations.</remarks>
        /// <returns>DataFormat property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataFormat GetDataFormat(DependencyObject element)
        {
            return (DataFormat)element.GetValue(DataFormatProperty);
        }

        /// <summary>Helper for setting <see cref="DataFormatProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DataFormatProperty"/> on.</param>
        /// <param name="value">DataFormat property value.</param>
        /// <remarks>Sets the data format which will be used for the drag and drop operations.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDataFormat(DependencyObject element, DataFormat value)
        {
            element.SetValue(DataFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty
            = DependencyProperty.RegisterAttached("IsDragSource",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false, OnIsDragSourceChanged));

        /// <summary>Helper for getting <see cref="IsDragSourceProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsDragSourceProperty"/> from.</param>
        /// <remarks>Gets whether the control can be used as drag source.</remarks>
        /// <returns>IsDragSource property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetIsDragSource(DependencyObject element)
        {
            return (bool)element.GetValue(IsDragSourceProperty);
        }

        /// <summary>Helper for setting <see cref="IsDragSourceProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsDragSourceProperty"/> on.</param>
        /// <param name="value">IsDragSource property value.</param>
        /// <remarks>Sets whether the control can be used as drag source.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetIsDragSource(DependencyObject element, bool value)
        {
            element.SetValue(IsDragSourceProperty, value);
        }

        private static void OnIsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourceOnMouseMove;

                uiElement.PreviewTouchDown -= DragSourceOnTouchDown;
                uiElement.PreviewTouchUp -= DragSourceOnTouchUp;
                uiElement.PreviewTouchMove -= DragSourceOnTouchMove;

                uiElement.QueryContinueDrag -= DragSourceOnQueryContinueDrag;
                uiElement.GiveFeedback -= DragSourceOnGiveFeedback;

                if ((bool)e.NewValue)
                {
                    uiElement.PreviewMouseLeftButtonDown += DragSourceOnMouseLeftButtonDown;
                    uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                    uiElement.PreviewMouseMove += DragSourceOnMouseMove;

                    uiElement.PreviewTouchDown += DragSourceOnTouchDown;
                    uiElement.PreviewTouchUp += DragSourceOnTouchUp;
                    uiElement.PreviewTouchMove += DragSourceOnTouchMove;

                    uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
                    uiElement.GiveFeedback += DragSourceOnGiveFeedback;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the control can be used as drop target.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty
            = DependencyProperty.RegisterAttached("IsDropTarget",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false, OnIsDropTargetChanged));

        /// <summary>Helper for getting <see cref="IsDropTargetProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsDropTargetProperty"/> from.</param>
        /// <remarks>Gets whether the control can be used as drop target.</remarks>
        /// <returns>IsDropTarget property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetIsDropTarget(DependencyObject element)
        {
            return (bool)element.GetValue(IsDropTargetProperty);
        }

        /// <summary>Helper for setting <see cref="IsDropTargetProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsDropTargetProperty"/> on.</param>
        /// <param name="value">IsDropTarget property value.</param>
        /// <remarks>Sets whether the control can be used as drop target.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetIsDropTarget(DependencyObject element, bool value)
        {
            element.SetValue(IsDropTargetProperty, value);
        }

        private static void OnIsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.AllowDrop = (bool)e.NewValue;

                UnregisterDragDropEvents(uiElement, GetDropEventType(d));

                if ((bool)e.NewValue)
                {
                    RegisterDragDropEvents(uiElement, GetDropEventType(d));
                }
            }
        }

        /// <summary>
        /// Gets or sets the events which are subscribed for the drag and drop events.
        /// </summary>
        public static readonly DependencyProperty DropEventTypeProperty
            = DependencyProperty.RegisterAttached("DropEventType",
                                                  typeof(EventType),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(EventType.Auto, OnDropEventTypeChanged));

        /// <summary>Helper for getting <see cref="DropEventTypeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropEventTypeProperty"/> from.</param>
        /// <remarks>Gets which type of events are subscribed for the drag and drop events.</remarks>
        /// <returns>DropEventType property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static EventType GetDropEventType(DependencyObject element)
        {
            return (EventType)element.GetValue(DropEventTypeProperty);
        }

        /// <summary>Helper for setting <see cref="DropEventTypeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropEventTypeProperty"/> on.</param>
        /// <param name="value">DropEventType property value.</param>
        /// <remarks>Sets which type of events are subscribed for the drag and drop events.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropEventType(DependencyObject element, EventType value)
        {
            element.SetValue(DropEventTypeProperty, value);
        }

        private static void OnDropEventTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                if (!GetIsDropTarget(uiElement))
                {
                    return;
                }

                UnregisterDragDropEvents(uiElement, (EventType)e.OldValue);
                RegisterDragDropEvents(uiElement, (EventType)e.NewValue);
            }
        }

        private static void RegisterDragDropEvents(UIElement uiElement, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Auto:
                    if (uiElement is ItemsControl)
                    {
                        // use normal events for ItemsControls
                        uiElement.DragEnter += DropTargetOnDragEnter;
                        uiElement.DragLeave += DropTargetOnDragLeave;
                        uiElement.DragOver += DropTargetOnDragOver;
                        uiElement.Drop += DropTargetOnDrop;
                        uiElement.GiveFeedback += DropTargetOnGiveFeedback;
                    }
                    else
                    {
                        // issue #85: try using preview events for all other elements than ItemsControls
                        uiElement.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                        uiElement.PreviewDragLeave += DropTargetOnDragLeave;
                        uiElement.PreviewDragOver += DropTargetOnPreviewDragOver;
                        uiElement.PreviewDrop += DropTargetOnPreviewDrop;
                        uiElement.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    }

                    break;

                case EventType.Tunneled:
                    uiElement.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                    uiElement.PreviewDragLeave += DropTargetOnDragLeave;
                    uiElement.PreviewDragOver += DropTargetOnPreviewDragOver;
                    uiElement.PreviewDrop += DropTargetOnPreviewDrop;
                    uiElement.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    break;

                case EventType.Bubbled:
                    uiElement.DragEnter += DropTargetOnDragEnter;
                    uiElement.DragLeave += DropTargetOnDragLeave;
                    uiElement.DragOver += DropTargetOnDragOver;
                    uiElement.Drop += DropTargetOnDrop;
                    uiElement.GiveFeedback += DropTargetOnGiveFeedback;
                    break;

                case EventType.TunneledBubbled:
                    uiElement.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                    uiElement.PreviewDragLeave += DropTargetOnDragLeave;
                    uiElement.PreviewDragOver += DropTargetOnPreviewDragOver;
                    uiElement.PreviewDrop += DropTargetOnPreviewDrop;
                    uiElement.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    uiElement.DragEnter += DropTargetOnDragEnter;
                    uiElement.DragLeave += DropTargetOnDragLeave;
                    uiElement.DragOver += DropTargetOnDragOver;
                    uiElement.Drop += DropTargetOnDrop;
                    uiElement.GiveFeedback += DropTargetOnGiveFeedback;
                    break;

                default:
                    throw new ArgumentException("Unknown value for eventType: " + eventType.ToString(), nameof(eventType));
            }
        }

        private static void UnregisterDragDropEvents(UIElement uiElement, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Auto:
                    if (uiElement is ItemsControl)
                    {
                        // use normal events for ItemsControls
                        uiElement.DragEnter -= DropTargetOnDragEnter;
                        uiElement.DragLeave -= DropTargetOnDragLeave;
                        uiElement.DragOver -= DropTargetOnDragOver;
                        uiElement.Drop -= DropTargetOnDrop;
                        uiElement.GiveFeedback -= DropTargetOnGiveFeedback;
                    }
                    else
                    {
                        // issue #85: try using preview events for all other elements than ItemsControls
                        uiElement.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                        uiElement.PreviewDragLeave -= DropTargetOnDragLeave;
                        uiElement.PreviewDragOver -= DropTargetOnPreviewDragOver;
                        uiElement.PreviewDrop -= DropTargetOnPreviewDrop;
                        uiElement.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    }

                    break;

                case EventType.Tunneled:
                    uiElement.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                    uiElement.PreviewDragLeave -= DropTargetOnDragLeave;
                    uiElement.PreviewDragOver -= DropTargetOnPreviewDragOver;
                    uiElement.PreviewDrop -= DropTargetOnPreviewDrop;
                    uiElement.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                case EventType.Bubbled:
                    uiElement.DragEnter -= DropTargetOnDragEnter;
                    uiElement.DragLeave -= DropTargetOnDragLeave;
                    uiElement.DragOver -= DropTargetOnDragOver;
                    uiElement.Drop -= DropTargetOnDrop;
                    uiElement.GiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                case EventType.TunneledBubbled:
                    uiElement.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                    uiElement.PreviewDragLeave -= DropTargetOnDragLeave;
                    uiElement.PreviewDragOver -= DropTargetOnPreviewDragOver;
                    uiElement.PreviewDrop -= DropTargetOnPreviewDrop;
                    uiElement.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    uiElement.DragEnter -= DropTargetOnDragEnter;
                    uiElement.DragLeave -= DropTargetOnDragLeave;
                    uiElement.DragOver -= DropTargetOnDragOver;
                    uiElement.Drop -= DropTargetOnDrop;
                    uiElement.GiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                default:
                    throw new ArgumentException("Unknown value for eventType: " + eventType.ToString(), nameof(eventType));
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Gets or sets whether the control can be used as drag source together with the right mouse.
        /// </summary>
        public static readonly DependencyProperty CanDragWithMouseRightButtonProperty
            = DependencyProperty.RegisterAttached("CanDragWithMouseRightButton",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false, OnCanDragWithMouseRightButtonChanged));

        /// <summary>Helper for getting <see cref="CanDragWithMouseRightButtonProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="CanDragWithMouseRightButtonProperty"/> from.</param>
        /// <remarks>Gets whether the control can be used as drag source together with the right mouse.</remarks>
        /// <returns>CanDragWithMouseRightButton property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetCanDragWithMouseRightButton(DependencyObject element)
        {
            return (bool)element.GetValue(CanDragWithMouseRightButtonProperty);
        }

        /// <summary>Helper for setting <see cref="CanDragWithMouseRightButtonProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CanDragWithMouseRightButtonProperty"/> on.</param>
        /// <param name="value">CanDragWithMouseRightButton property value.</param>
        /// <remarks>Sets whether the control can be used as drag source together with the right mouse.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetCanDragWithMouseRightButton(DependencyObject element, bool value)
        {
            element.SetValue(CanDragWithMouseRightButtonProperty, value);
        }

        private static void OnCanDragWithMouseRightButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.PreviewMouseRightButtonDown -= DragSourceOnMouseRightButtonDown;
                uiElement.PreviewMouseRightButtonUp -= DragSourceOnMouseRightButtonUp;

                if ((bool)e.NewValue)
                {
                    uiElement.PreviewMouseRightButtonDown += DragSourceOnMouseRightButtonDown;
                    uiElement.PreviewMouseRightButtonUp += DragSourceOnMouseRightButtonUp;
                }
            }
        }

        /// <summary>
        /// Gets or sets the handler for the drag operation.
        /// </summary>
        public static readonly DependencyProperty DragHandlerProperty
            = DependencyProperty.RegisterAttached("DragHandler",
                                                  typeof(IDragSource),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DragHandlerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragHandlerProperty"/> from.</param>
        /// <remarks>Gets the handler for the drag operation.</remarks>
        /// <returns>DragHandler property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDragSource GetDragHandler(DependencyObject element)
        {
            return (IDragSource)element.GetValue(DragHandlerProperty);
        }

        /// <summary>Helper for setting <see cref="DragHandlerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragHandlerProperty"/> on.</param>
        /// <param name="value">DragHandler property value.</param>
        /// <remarks>Sets the handler for the drag operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragHandler(DependencyObject element, IDragSource value)
        {
            element.SetValue(DragHandlerProperty, value);
        }

        /// <summary>
        /// Gets or sets the handler for the drop operation.
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty
            = DependencyProperty.RegisterAttached("DropHandler",
                                                  typeof(IDropTarget),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DropHandlerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropHandlerProperty"/> from.</param>
        /// <remarks>Gets the handler for the drop operation.</remarks>
        /// <returns>DropHandler property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDropTarget GetDropHandler(DependencyObject element)
        {
            return (IDropTarget)element.GetValue(DropHandlerProperty);
        }

        /// <summary>Helper for setting <see cref="DropHandlerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropHandlerProperty"/> on.</param>
        /// <param name="value">DropHandler property value.</param>
        /// <remarks>Sets the handler for the drop operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropHandler(DependencyObject element, IDropTarget value)
        {
            element.SetValue(DropHandlerProperty, value);
        }

        /// <summary>
        /// Gets or sets the drag info builder for the drag operation.
        /// </summary>
        public static readonly DependencyProperty DragInfoBuilderProperty
            = DependencyProperty.RegisterAttached("DragInfoBuilder",
                                                  typeof(IDragInfoBuilder),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DragInfoBuilderProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragInfoBuilderProperty"/> from.</param>
        /// <remarks>Gets the drag info builder for the drag operation.</remarks>
        /// <returns>DragInfoBuilder property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDragInfoBuilder GetDragInfoBuilder(DependencyObject element)
        {
            return (IDragInfoBuilder)element.GetValue(DragInfoBuilderProperty);
        }

        /// <summary>Helper for setting <see cref="DragInfoBuilderProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragInfoBuilderProperty"/> on.</param>
        /// <param name="value">DragInfoBuilder property value.</param>
        /// <remarks>Sets the drag info builder for the drag operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragInfoBuilder(DependencyObject element, IDragInfoBuilder value)
        {
            element.SetValue(DragInfoBuilderProperty, value);
        }

        /// <summary>
        /// Gets or sets the drop info builder for the drop operation.
        /// </summary>
        public static readonly DependencyProperty DropInfoBuilderProperty
            = DependencyProperty.RegisterAttached("DropInfoBuilder",
                                                  typeof(IDropInfoBuilder),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DropInfoBuilderProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropInfoBuilderProperty"/> from.</param>
        /// <remarks>Gets the drop info builder for the drop operation.</remarks>
        /// <returns>DropInfoBuilder property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDropInfoBuilder GetDropInfoBuilder(DependencyObject element)
        {
            return (IDropInfoBuilder)element.GetValue(DropInfoBuilderProperty);
        }

        /// <summary>Helper for setting <see cref="DropInfoBuilderProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropInfoBuilderProperty"/> on.</param>
        /// <param name="value">DropInfoBuilder property value.</param>
        /// <remarks>Sets the drop info builder for the drop operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropInfoBuilder(DependencyObject element, IDropInfoBuilder value)
        {
            element.SetValue(DropInfoBuilderProperty, value);
        }

        /// <summary>
        /// Gets or sets the ScrollingMode for the drop operation.
        /// </summary>
        public static readonly DependencyProperty DropScrollingModeProperty
            = DependencyProperty.RegisterAttached("DropScrollingMode",
                                                  typeof(ScrollingMode),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(ScrollingMode.Both));

        /// <summary>Helper for getting <see cref="DropScrollingModeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropScrollingModeProperty"/> from.</param>
        /// <remarks>Gets the ScrollingMode for the drop operation.</remarks>
        /// <returns>DropScrollingMode property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ScrollingMode GetDropScrollingMode(DependencyObject element)
        {
            return (ScrollingMode)element.GetValue(DropScrollingModeProperty);
        }

        /// <summary>Helper for setting <see cref="DropScrollingModeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropScrollingModeProperty"/> on.</param>
        /// <param name="value">DropScrollingMode property value.</param>
        /// <remarks>Sets the ScrollingMode for the drop operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropScrollingMode(DependencyObject element, ScrollingMode value)
        {
            element.SetValue(DropScrollingModeProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show the DropTargetAdorner (DropTargetInsertionAdorner) on an empty target too.
        /// </summary>
        public static readonly DependencyProperty ShowAlwaysDropTargetAdornerProperty
            = DependencyProperty.RegisterAttached("ShowAlwaysDropTargetAdorner",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="ShowAlwaysDropTargetAdornerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShowAlwaysDropTargetAdornerProperty"/> from.</param>
        /// <remarks>Gets whether to show the DropTargetAdorner (DropTargetInsertionAdorner) on an empty target too.</remarks>
        /// <returns>ShowAlwaysDropTargetAdorner property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetShowAlwaysDropTargetAdorner(DependencyObject element)
        {
            return (bool)element.GetValue(ShowAlwaysDropTargetAdornerProperty);
        }

        /// <summary>Helper for setting <see cref="ShowAlwaysDropTargetAdornerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShowAlwaysDropTargetAdornerProperty"/> on.</param>
        /// <param name="value">ShowAlwaysDropTargetAdorner property value.</param>
        /// <remarks>Sets whether to show the DropTargetAdorner (DropTargetInsertionAdorner) on an empty target too.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetShowAlwaysDropTargetAdorner(DependencyObject element, bool value)
        {
            element.SetValue(ShowAlwaysDropTargetAdornerProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush for the DropTargetAdorner.
        /// </summary>
        public static readonly DependencyProperty DropTargetAdornerBrushProperty
            = DependencyProperty.RegisterAttached("DropTargetAdornerBrush",
                                                  typeof(Brush),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((Brush)null));

        /// <summary>Helper for getting <see cref="DropTargetAdornerBrushProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropTargetAdornerBrushProperty"/> from.</param>
        /// <remarks>Gets the brush for the DropTargetAdorner.</remarks>
        /// <returns>DropTargetAdornerBrush property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Brush GetDropTargetAdornerBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(DropTargetAdornerBrushProperty);
        }

        /// <summary>Helper for setting <see cref="DropTargetAdornerBrushProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropTargetAdornerBrushProperty"/> on.</param>
        /// <param name="value">DropTargetAdornerBrush property value.</param>
        /// <remarks>Sets the brush for the DropTargetAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropTargetAdornerBrush(DependencyObject element, Brush value)
        {
            element.SetValue(DropTargetAdornerBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the pen for the DropTargetAdorner.
        /// </summary>
        public static readonly DependencyProperty DropTargetAdornerPenProperty
            = DependencyProperty.RegisterAttached("DropTargetAdornerPen",
                                                  typeof(Pen),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((Pen)null));

        /// <summary>Helper for getting <see cref="DropTargetAdornerPenProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropTargetAdornerPenProperty"/> from.</param>
        /// <remarks>Gets the pen for the DropTargetAdorner.</remarks>
        /// <returns>DropTargetAdornerPen property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Pen GetDropTargetAdornerPen(DependencyObject element)
        {
            return (Pen)element.GetValue(DropTargetAdornerPenProperty);
        }

        /// <summary>Helper for setting <see cref="DropTargetAdornerPenProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropTargetAdornerPenProperty"/> on.</param>
        /// <param name="value">DropTargetAdornerPen property value.</param>
        /// <remarks>Sets the pen for the DropTargetAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropTargetAdornerPen(DependencyObject element, Pen value)
        {
            element.SetValue(DropTargetAdornerPenProperty, value);
        }

        /// <summary>
        /// Gets or sets a context for a control. Only controls with the same context are allowed for drag or drop actions.
        /// </summary>
        public static readonly DependencyProperty DragDropContextProperty
            = DependencyProperty.RegisterAttached("DragDropContext",
                                                  typeof(string),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(string.Empty));

        /// <summary>Helper for getting <see cref="DragDropContextProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragDropContextProperty"/> from.</param>
        /// <remarks>Gets a context for a control. Only controls with the same context are allowed for drag or drop actions.</remarks>
        /// <returns>DragDropContext property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static string GetDragDropContext(DependencyObject element)
        {
            return (string)element.GetValue(DragDropContextProperty);
        }

        /// <summary>Helper for setting <see cref="DragDropContextProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragDropContextProperty"/> on.</param>
        /// <param name="value">DragDropContext property value.</param>
        /// <remarks>Sets a context for a control. Only controls with the same context are allowed for drag or drop actions.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragDropContext(DependencyObject element, string value)
        {
            element.SetValue(DragDropContextProperty, value);
        }

        /// <summary>
        /// Gets or sets whether an element under the mouse should be ignored for the drag operation.
        /// </summary>
        public static readonly DependencyProperty DragSourceIgnoreProperty
            = DependencyProperty.RegisterAttached("DragSourceIgnore",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Helper for getting <see cref="DragSourceIgnoreProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragSourceIgnoreProperty"/> from.</param>
        /// <remarks>Gets whether an element under the mouse should be ignored for the drag operation.</remarks>
        /// <returns>DragSourceIgnore property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetDragSourceIgnore(DependencyObject element)
        {
            return (bool)element.GetValue(DragSourceIgnoreProperty);
        }

        /// <summary>Helper for setting <see cref="DragSourceIgnoreProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragSourceIgnoreProperty"/> on.</param>
        /// <param name="value">DragSourceIgnore property value.</param>
        /// <remarks>Sets whether an element under the mouse should be ignored for the drag operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragSourceIgnore(DependencyObject element, bool value)
        {
            element.SetValue(DragSourceIgnoreProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the drag action should be started only directly on a selected item.
        /// or also on the free control space (e.g. in a ListBox).
        /// </summary>
        public static readonly DependencyProperty DragDirectlySelectedOnlyProperty
            = DependencyProperty.RegisterAttached("DragDirectlySelectedOnly",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="DragDirectlySelectedOnlyProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragDirectlySelectedOnlyProperty"/> from.</param>
        /// <remarks>Gets whether the drag action should be started only directly on a selected item.</remarks>
        /// <returns>DragDirectlySelectedOnly property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetDragDirectlySelectedOnly(DependencyObject element)
        {
            return (bool)element.GetValue(DragDirectlySelectedOnlyProperty);
        }

        /// <summary>Helper for setting <see cref="DragDirectlySelectedOnlyProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragDirectlySelectedOnlyProperty"/> on.</param>
        /// <param name="value">DragDirectlySelectedOnly property value.</param>
        /// <remarks>Sets whether the drag action should be started only directly on a selected item.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragDirectlySelectedOnly(DependencyObject element, bool value)
        {
            element.SetValue(DragDirectlySelectedOnlyProperty, value);
        }

        /// <summary>
        /// The drag drop copy key state property (default None).
        /// So the drag drop action is
        /// - Move, within the same control or from one to another, if the drag drop key state is None
        /// - Copy, from one to another control with the given drag drop copy key state
        /// </summary>
        public static readonly DependencyProperty DragDropCopyKeyStateProperty
            = DependencyProperty.RegisterAttached("DragDropCopyKeyState",
                                                  typeof(DragDropKeyStates),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(DragDropKeyStates)));

        /// <summary>Helper for getting <see cref="DragDropCopyKeyStateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragDropCopyKeyStateProperty"/> from.</param>
        /// <remarks>Gets the copy key state which indicates the effect of the drag drop operation.</remarks>
        /// <returns>DragDropCopyKeyState property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DragDropKeyStates GetDragDropCopyKeyState(DependencyObject element)
        {
            return (DragDropKeyStates)element.GetValue(DragDropCopyKeyStateProperty);
        }

        /// <summary>Helper for setting <see cref="DragDropCopyKeyStateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragDropCopyKeyStateProperty"/> on.</param>
        /// <param name="value">DragDropCopyKeyState property value.</param>
        /// <remarks>Sets the copy key state which indicates the effect of the drag drop operation.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragDropCopyKeyState(DependencyObject element, DragDropKeyStates value)
        {
            element.SetValue(DragDropCopyKeyStateProperty, value);
        }

        /// <summary>
        /// Gets or sets whether if the default DragAdorner should be use.
        /// </summary>
        public static readonly DependencyProperty UseDefaultDragAdornerProperty
            = DependencyProperty.RegisterAttached("UseDefaultDragAdorner",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="UseDefaultDragAdornerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="UseDefaultDragAdornerProperty"/> from.</param>
        /// <remarks>Gets whether if the default DragAdorner is used.</remarks>
        /// <returns>UseDefaultDragAdorner property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetUseDefaultDragAdorner(DependencyObject element)
        {
            return (bool)element.GetValue(UseDefaultDragAdornerProperty);
        }

        /// <summary>Helper for setting <see cref="UseDefaultDragAdornerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="UseDefaultDragAdornerProperty"/> on.</param>
        /// <param name="value">UseDefaultDragAdorner property value.</param>
        /// <remarks>Sets whether if the default DragAdorner should be use.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetUseDefaultDragAdorner(DependencyObject element, bool value)
        {
            element.SetValue(UseDefaultDragAdornerProperty, value);
        }

        /// <summary>
        /// Gets or sets the opacity of the default DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DefaultDragAdornerOpacityProperty
            = DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity",
                                                  typeof(double),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(0.8));

        /// <summary>Helper for getting <see cref="DefaultDragAdornerOpacityProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DefaultDragAdornerOpacityProperty"/> from.</param>
        /// <remarks>Gets the opacity of the default DragAdorner.</remarks>
        /// <returns>DefaultDragAdornerOpacity property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static double GetDefaultDragAdornerOpacity(DependencyObject element)
        {
            return (double)element.GetValue(DefaultDragAdornerOpacityProperty);
        }

        /// <summary>Helper for setting <see cref="DefaultDragAdornerOpacityProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DefaultDragAdornerOpacityProperty"/> on.</param>
        /// <param name="value">DefaultDragAdornerOpacity property value.</param>
        /// <remarks>Sets the opacity of the default DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDefaultDragAdornerOpacity(DependencyObject element, double value)
        {
            element.SetValue(DefaultDragAdornerOpacityProperty, value);
        }

        /// <summary>
        /// Gets or sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragMouseAnchorPointProperty
            = DependencyProperty.RegisterAttached("DragMouseAnchorPoint",
                                                  typeof(Point),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(new Point(0, 1)));

        /// <summary>Helper for getting <see cref="DragMouseAnchorPointProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragMouseAnchorPointProperty"/> from.</param>
        /// <remarks>Gets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.</remarks>
        /// <returns>DragMouseAnchorPoint property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetDragMouseAnchorPoint(DependencyObject element)
        {
            return (Point)element.GetValue(DragMouseAnchorPointProperty);
        }

        /// <summary>Helper for setting <see cref="DragMouseAnchorPointProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragMouseAnchorPointProperty"/> on.</param>
        /// <param name="value">DragMouseAnchorPoint property value.</param>
        /// <remarks>Sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragMouseAnchorPoint(DependencyObject element, Point value)
        {
            element.SetValue(DragMouseAnchorPointProperty, value);
        }

        /// <summary>
        /// Gets or sets the translation transform which will be used for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTranslationProperty
            = DependencyProperty.RegisterAttached("DragAdornerTranslation",
                                                  typeof(Point),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(new Point(-4, -4)));

        /// <summary>Helper for getting <see cref="DragAdornerTranslationProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerTranslationProperty"/> from.</param>
        /// <remarks>Gets the translation transform which will be used for the DragAdorner.</remarks>
        /// <returns>DragAdornerTranslation property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetDragAdornerTranslation(DependencyObject element)
        {
            return (Point)element.GetValue(DragAdornerTranslationProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerTranslationProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerTranslationProperty"/> on.</param>
        /// <param name="value">DragAdornerTranslation property value.</param>
        /// <remarks>Sets the translation transform which will be used for the DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerTranslation(DependencyObject element, Point value)
        {
            element.SetValue(DragAdornerTranslationProperty, value);
        }

        /// <summary>
        /// Gets or sets the translation transform which will be used for the EffectAdorner.
        /// </summary>
        public static readonly DependencyProperty EffectAdornerTranslationProperty
            = DependencyProperty.RegisterAttached("EffectAdornerTranslation",
                                                  typeof(Point),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(new Point(16, 16)));

        /// <summary>Helper for getting <see cref="EffectAdornerTranslationProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectAdornerTranslationProperty"/> from.</param>
        /// <remarks>Gets the translation transform which will be used for the EffectAdorner.</remarks>
        /// <returns>EffectAdornerTranslation property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetEffectAdornerTranslation(DependencyObject element)
        {
            return (Point)element.GetValue(EffectAdornerTranslationProperty);
        }

        /// <summary>Helper for setting <see cref="EffectAdornerTranslationProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectAdornerTranslationProperty"/> on.</param>
        /// <param name="value">EffectAdornerTranslation property value.</param>
        /// <remarks>Sets the translation transform which will be used for the EffectAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectAdornerTranslation(DependencyObject element, Point value)
        {
            element.SetValue(EffectAdornerTranslationProperty, value);
        }

        /// <summary>
        /// Gets or sets a DataTemplate for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("DragAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DragAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets the DataTemplate for the DragAdorner.</remarks>
        /// <returns>DragAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetDragAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(DragAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerTemplateProperty"/> on.</param>
        /// <param name="value">DragAdornerTemplate property value.</param>
        /// <remarks>Sets the DataTemplate for the DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(DragAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a DataTemplateSelector for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DragAdornerTemplateSelector",
                                                  typeof(DataTemplateSelector),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>Helper for getting <see cref="DragAdornerTemplateSelectorProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerTemplateSelectorProperty"/> from.</param>
        /// <remarks>Gets the DataTemplateSelector for the DragAdorner.</remarks>
        /// <returns>DragAdornerTemplateSelector property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerTemplateSelectorProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerTemplateSelectorProperty"/> on.</param>
        /// <param name="value">DragAdornerTemplateSelector property value.</param>
        /// <remarks>Sets the DataTemplateSelector for the DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets a DragAdorner DataTemplate for multiple item selection.
        /// </summary>
        public static readonly DependencyProperty DragAdornerMultiItemTemplateProperty
            = DependencyProperty.RegisterAttached("DragAdornerMultiItemTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DragAdornerMultiItemTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerMultiItemTemplateProperty"/> from.</param>
        /// <remarks>Gets the DragAdorner DataTemplate for multiple item selection.</remarks>
        /// <returns>DragAdornerMultiItemTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetDragAdornerMultiItemTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(DragAdornerMultiItemTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerMultiItemTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerMultiItemTemplateProperty"/> on.</param>
        /// <param name="value">DragAdornerMultiItemTemplate property value.</param>
        /// <remarks>Sets the DragAdorner DataTemplate for multiple item selection.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerMultiItemTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(DragAdornerMultiItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a DragAdorner DataTemplateSelector for multiple item selection.
        /// </summary>
        public static readonly DependencyProperty DragAdornerMultiItemTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DragAdornerMultiItemTemplateSelector",
                                                  typeof(DataTemplateSelector),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>Helper for getting <see cref="DragAdornerMultiItemTemplateSelectorProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerMultiItemTemplateSelectorProperty"/> from.</param>
        /// <remarks>Gets the DragAdorner DataTemplateSelector for multiple item selection.</remarks>
        /// <returns>DragAdornerMultiItemTemplateSelector property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplateSelector GetDragAdornerMultiItemTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerMultiItemTemplateSelectorProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerMultiItemTemplateSelectorProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerMultiItemTemplateSelectorProperty"/> on.</param>
        /// <param name="value">DragAdornerMultiItemTemplateSelector property value.</param>
        /// <remarks>Sets the DragAdorner DataTemplateSelector for multiple item selection.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerMultiItemTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerMultiItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets a ItemsPanel for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerItemsPanelProperty
            = DependencyProperty.RegisterAttached("DragAdornerItemsPanel",
                                                  typeof(ItemsPanelTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(ItemsControl.ItemsPanelProperty.DefaultMetadata.DefaultValue));

        /// <summary>Helper for getting <see cref="DragAdornerItemsPanelProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragAdornerItemsPanelProperty"/> from.</param>
        /// <remarks>Gets the ItemsPanel for the DragAdorner.</remarks>
        /// <returns>DragAdornerItemsPanel property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ItemsPanelTemplate GetDragAdornerItemsPanel(DependencyObject element)
        {
            return (ItemsPanelTemplate)element.GetValue(DragAdornerItemsPanelProperty);
        }

        /// <summary>Helper for setting <see cref="DragAdornerItemsPanelProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragAdornerItemsPanelProperty"/> on.</param>
        /// <param name="value">DragAdornerItemsPanel property value.</param>
        /// <remarks>Sets the ItemsPanel for the DragAdorner.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragAdornerItemsPanel(DependencyObject element, ItemsPanelTemplate value)
        {
            element.SetValue(DragAdornerItemsPanelProperty, value);
        }

        /// <summary>
        /// Gets or sets a DataTemplate for the DragAdorner based on the DropTarget.
        /// </summary>
        public static readonly DependencyProperty DropAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("DropAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DropAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets the DataTemplate for the DragAdorner based on the DropTarget.</remarks>
        /// <returns>DropAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetDropAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(DropAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="DropAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropAdornerTemplateProperty"/> on.</param>
        /// <param name="value">DropAdornerTemplate property value.</param>
        /// <remarks>Sets the DataTemplate for the DragAdorner based on the DropTarget.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(DropAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a DataTemplateSelector for the DragAdorner based on the DropTarget.
        /// </summary>
        public static readonly DependencyProperty DropAdornerTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DropAdornerTemplateSelector",
                                                  typeof(DataTemplateSelector),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>Helper for getting <see cref="DropAdornerTemplateSelectorProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropAdornerTemplateSelectorProperty"/> from.</param>
        /// <remarks>Gets the DataTemplateSelector for the DragAdorner based on the DropTarget.</remarks>
        /// <returns>DropAdornerTemplateSelector property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplateSelector GetDropAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DropAdornerTemplateSelectorProperty);
        }

        /// <summary>Helper for setting <see cref="DropAdornerTemplateSelectorProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropAdornerTemplateSelectorProperty"/> on.</param>
        /// <param name="value">DropAdornerTemplateSelector property value.</param>
        /// <remarks>Sets the DataTemplateSelector for the DragAdorner based on the DropTarget.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DropAdornerTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets a DropAdorner DataTemplate for multiple item selection.
        /// </summary>
        public static readonly DependencyProperty DropAdornerMultiItemTemplateProperty
            = DependencyProperty.RegisterAttached("DropAdornerMultiItemTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="DropAdornerMultiItemTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropAdornerMultiItemTemplateProperty"/> from.</param>
        /// <remarks>Gets the DropAdorner DataTemplate for multiple item selection.</remarks>
        /// <returns>DropAdornerMultiItemTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetDropAdornerMultiItemTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(DropAdornerMultiItemTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="DropAdornerMultiItemTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropAdornerMultiItemTemplateProperty"/> on.</param>
        /// <param name="value">DropAdornerMultiItemTemplate property value.</param>
        /// <remarks>Sets the DropAdorner DataTemplate for multiple item selection.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropAdornerMultiItemTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(DropAdornerMultiItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a DropAdorner DataTemplateSelector for multiple item selection.
        /// </summary>
        public static readonly DependencyProperty DropAdornerMultiItemTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DropAdornerMultiItemTemplateSelector",
                                                  typeof(DataTemplateSelector),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>Helper for getting <see cref="DropAdornerMultiItemTemplateSelectorProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropAdornerMultiItemTemplateSelectorProperty"/> from.</param>
        /// <remarks>Gets the DropAdorner DataTemplateSelector for multiple item selection.</remarks>
        /// <returns>DropAdornerMultiItemTemplateSelector property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplateSelector GetDropAdornerMultiItemTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DropAdornerMultiItemTemplateSelectorProperty);
        }

        /// <summary>Helper for setting <see cref="DropAdornerMultiItemTemplateSelectorProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropAdornerMultiItemTemplateSelectorProperty"/> on.</param>
        /// <param name="value">DropAdornerMultiItemTemplateSelector property value.</param>
        /// <remarks>Sets the DropAdorner DataTemplateSelector for multiple item selection.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropAdornerMultiItemTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DropAdornerMultiItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets a ItemsPanel for the DragAdorner based on the DropTarget.
        /// </summary>
        public static readonly DependencyProperty DropAdornerItemsPanelProperty
            = DependencyProperty.RegisterAttached("DropAdornerItemsPanel",
                                                  typeof(ItemsPanelTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(ItemsControl.ItemsPanelProperty.DefaultMetadata.DefaultValue));

        /// <summary>Helper for getting <see cref="DropAdornerItemsPanelProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropAdornerItemsPanelProperty"/> from.</param>
        /// <remarks>Gets the ItemsPanel for the DragAdorner based on the DropTarget.</remarks>
        /// <returns>DropAdornerItemsPanel property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ItemsPanelTemplate GetDropAdornerItemsPanel(DependencyObject element)
        {
            return (ItemsPanelTemplate)element.GetValue(DropAdornerItemsPanelProperty);
        }

        /// <summary>Helper for setting <see cref="DropAdornerItemsPanelProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropAdornerItemsPanelProperty"/> on.</param>
        /// <param name="value">DropAdornerItemsPanel property value.</param>
        /// <remarks>Sets the ItemsPanel for the DragAdorner based on the DropTarget.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropAdornerItemsPanel(DependencyObject element, ItemsPanelTemplate value)
        {
            element.SetValue(DropAdornerItemsPanelProperty, value);
        }

        /// <summary>
        /// Use descendant bounds of the VisualSourceItem as MinWidth for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty UseVisualSourceItemSizeForDragAdornerProperty
            = DependencyProperty.RegisterAttached("UseVisualSourceItemSizeForDragAdorner",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="UseVisualSourceItemSizeForDragAdornerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="UseVisualSourceItemSizeForDragAdornerProperty"/> from.</param>
        /// <remarks>Gets the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.</remarks>
        /// <returns>UseVisualSourceItemSizeForDragAdorner property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetUseVisualSourceItemSizeForDragAdorner(DependencyObject element)
        {
            return (bool)element.GetValue(UseVisualSourceItemSizeForDragAdornerProperty);
        }

        /// <summary>Helper for setting <see cref="UseVisualSourceItemSizeForDragAdornerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="UseVisualSourceItemSizeForDragAdornerProperty"/> on.</param>
        /// <param name="value">UseVisualSourceItemSizeForDragAdorner property value.</param>
        /// <remarks>Sets the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetUseVisualSourceItemSizeForDragAdorner(DependencyObject element, bool value)
        {
            element.SetValue(UseVisualSourceItemSizeForDragAdornerProperty, value);
        }

        /// <summary>
        /// Gets or sets whether if the default DataTemplate for the effects should be use.
        /// </summary>
        public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty
            = DependencyProperty.RegisterAttached("UseDefaultEffectDataTemplate",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="UseDefaultEffectDataTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="UseDefaultEffectDataTemplateProperty"/> from.</param>
        /// <remarks>Gets whether if the default DataTemplate for the effects should be use.</remarks>
        /// <returns>UseDefaultEffectDataTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetUseDefaultEffectDataTemplate(DependencyObject element)
        {
            return (bool)element.GetValue(UseDefaultEffectDataTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="UseDefaultEffectDataTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="UseDefaultEffectDataTemplateProperty"/> on.</param>
        /// <param name="value">UseDefaultEffectDataTemplate property value.</param>
        /// <remarks>Sets whether if the default DataTemplate for the effects should be use.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetUseDefaultEffectDataTemplate(DependencyObject element, bool value)
        {
            element.SetValue(UseDefaultEffectDataTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type None.
        /// </summary>
        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectNoneAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectNoneAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type None.</remarks>
        /// <returns>EffectNoneAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectNoneAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectNoneAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectNoneAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectNoneAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectNoneAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type None.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectNoneAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectNoneAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type Copy.
        /// </summary>
        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectCopyAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectCopyAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type Copy.</remarks>
        /// <returns>EffectCopyAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectCopyAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectCopyAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectCopyAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectCopyAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectCopyAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type Copy.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectCopyAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectCopyAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type Move.
        /// </summary>
        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectMoveAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectMoveAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type Move.</remarks>
        /// <returns>EffectMoveAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectMoveAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectMoveAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectMoveAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectMoveAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectMoveAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type Move.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectMoveAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectMoveAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type Link.
        /// </summary>
        public static readonly DependencyProperty EffectLinkAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectLinkAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectLinkAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectLinkAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type Link.</remarks>
        /// <returns>EffectLinkAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectLinkAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectLinkAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectLinkAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectLinkAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectLinkAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type Link.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectLinkAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectLinkAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type All.
        /// </summary>
        public static readonly DependencyProperty EffectAllAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectAllAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectAllAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectAllAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type All.</remarks>
        /// <returns>EffectAllAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectAllAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectAllAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectAllAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectAllAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectAllAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type All.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectAllAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectAllAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a EffectAdorner DataTemplate for effect type Scroll.
        /// </summary>
        public static readonly DependencyProperty EffectScrollAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("EffectScrollAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((DataTemplate)null));

        /// <summary>Helper for getting <see cref="EffectScrollAdornerTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EffectScrollAdornerTemplateProperty"/> from.</param>
        /// <remarks>Gets a EffectAdorner DataTemplate for effect type Scroll.</remarks>
        /// <returns>EffectScrollAdornerTemplate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static DataTemplate GetEffectScrollAdornerTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(EffectScrollAdornerTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="EffectScrollAdornerTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EffectScrollAdornerTemplateProperty"/> on.</param>
        /// <param name="value">EffectScrollAdornerTemplate property value.</param>
        /// <remarks>Sets a EffectAdorner DataTemplate for effect type Scroll.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetEffectScrollAdornerTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(EffectScrollAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientation of the inner ItemsPanel,
        /// but sometimes it's necessary to force the orientation, if the look up is wrong.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelOrientationProperty
            = DependencyProperty.RegisterAttached("ItemsPanelOrientation",
                                                  typeof(Orientation?),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(null));

        /// <summary>Helper for getting <see cref="ItemsPanelOrientationProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ItemsPanelOrientationProperty"/> from.</param>
        /// <remarks>
        /// Gets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientation of the inner ItemsPanel,
        /// but sometimes it's necessary to force the orientation, if the look up is wrong.
        /// </remarks>
        /// <returns>ItemsPanelOrientation property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Orientation? GetItemsPanelOrientation(DependencyObject element)
        {
            return (Orientation?)element.GetValue(ItemsPanelOrientationProperty);
        }

        /// <summary>Helper for setting <see cref="ItemsPanelOrientationProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ItemsPanelOrientationProperty"/> on.</param>
        /// <param name="value">ItemsPanelOrientation property value.</param>
        /// <remarks>
        /// Sets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientation of the inner ItemsPanel,
        /// but sometimes it's necessary to force the orientation, if the look up is wrong.
        /// </remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetItemsPanelOrientation(DependencyObject element, Orientation? value)
        {
            element.SetValue(ItemsPanelOrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum horizontal drag distance to allow for limited movement of the mouse pointer before a drag operation begins.
        /// Default is SystemParameters.MinimumHorizontalDragDistance.
        /// </summary>
        public static readonly DependencyProperty MinimumHorizontalDragDistanceProperty
            = DependencyProperty.RegisterAttached("MinimumHorizontalDragDistance",
                                                  typeof(double),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(SystemParameters.MinimumHorizontalDragDistance));

        /// <summary>Helper for getting <see cref="MinimumHorizontalDragDistanceProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="MinimumHorizontalDragDistanceProperty"/> from.</param>
        /// <remarks>Gets the minimum horizontal drag distance.</remarks>
        /// <returns>MinimumHorizontalDragDistance property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static double GetMinimumHorizontalDragDistance(DependencyObject element)
        {
            return (double)element.GetValue(MinimumHorizontalDragDistanceProperty);
        }

        /// <summary>Helper for setting <see cref="MinimumHorizontalDragDistanceProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="MinimumHorizontalDragDistanceProperty"/> on.</param>
        /// <param name="value">MinimumHorizontalDragDistance property value.</param>
        /// <remarks>Sets the minimum horizontal drag distance.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetMinimumHorizontalDragDistance(DependencyObject element, double value)
        {
            element.SetValue(MinimumHorizontalDragDistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum vertical drag distance to allow for limited movement of the mouse pointer before a drag operation begins.
        /// Default is SystemParameters.MinimumVerticalDragDistance.
        /// </summary>
        public static readonly DependencyProperty MinimumVerticalDragDistanceProperty
            = DependencyProperty.RegisterAttached("MinimumVerticalDragDistance",
                                                  typeof(double),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(SystemParameters.MinimumVerticalDragDistance));

        /// <summary>Helper for getting <see cref="MinimumVerticalDragDistanceProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="MinimumVerticalDragDistanceProperty"/> from.</param>
        /// <remarks>Gets the minimum vertical drag distance.</remarks>
        /// <returns>MinimumVerticalDragDistance property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static double GetMinimumVerticalDragDistance(DependencyObject element)
        {
            return (double)element.GetValue(MinimumVerticalDragDistanceProperty);
        }

        /// <summary>Helper for setting <see cref="MinimumVerticalDragDistanceProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="MinimumVerticalDragDistanceProperty"/> on.</param>
        /// <param name="value">MinimumVerticalDragDistance property value.</param>
        /// <remarks>Sets the minimum vertical drag distance.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetMinimumVerticalDragDistance(DependencyObject element, double value)
        {
            element.SetValue(MinimumVerticalDragDistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets whether if the dropped items should be select again (should keep the selection).
        /// Default is false.
        /// </summary>
        public static readonly DependencyProperty SelectDroppedItemsProperty
            = DependencyProperty.RegisterAttached("SelectDroppedItems",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>Helper for getting <see cref="SelectDroppedItemsProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="SelectDroppedItemsProperty"/> from.</param>
        /// <remarks>Gets whether if the dropped items should be select again (should keep the selection).</remarks>
        /// <returns>SelectDroppedItems property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetSelectDroppedItems(DependencyObject element)
        {
            return (bool)element.GetValue(SelectDroppedItemsProperty);
        }

        /// <summary>Helper for setting <see cref="SelectDroppedItemsProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="SelectDroppedItemsProperty"/> on.</param>
        /// <param name="value">SelectDroppedItems property value.</param>
        /// <remarks>Sets whether if the dropped items should be select again (should keep the selection).</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetSelectDroppedItems(DependencyObject element, bool value)
        {
            element.SetValue(SelectDroppedItemsProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ScrollViewer"/> that will be used as <see cref="DropInfo.TargetScrollViewer"/>.
        /// </summary>
        public static readonly DependencyProperty DropTargetScrollViewerProperty
            = DependencyProperty.RegisterAttached("DropTargetScrollViewer",
                                                  typeof(ScrollViewer),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata((ScrollViewer)null));

        /// <summary>Helper for getting <see cref="DropTargetScrollViewerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropTargetScrollViewerProperty"/> from.</param>
        /// <remarks>Gets the <see cref="ScrollViewer"/> that will be used as <see cref="DropInfo.TargetScrollViewer"/>.</remarks>
        /// <returns>DropTargetScrollViewer property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ScrollViewer GetDropTargetScrollViewer(DependencyObject element)
        {
            return (ScrollViewer)element?.GetValue(DropTargetScrollViewerProperty);
        }

        /// <summary>Helper for setting <see cref="DropTargetScrollViewerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropTargetScrollViewerProperty"/> on.</param>
        /// <param name="value">DropTargetScrollViewer property value.</param>
        /// <remarks>Sets the <see cref="ScrollViewer"/> that will be used as <see cref="DropInfo.TargetScrollViewer"/>.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropTargetScrollViewer(DependencyObject element, ScrollViewer value)
        {
            element.SetValue(DropTargetScrollViewerProperty, value);
        }

        /// <summary>
        /// Gets or sets the root element finder.
        /// </summary>
        public static readonly DependencyProperty RootElementFinderProperty
            = DependencyProperty.RegisterAttached("RootElementFinder",
                                                  typeof(IRootElementFinder),
                                                  typeof(DragDrop));

        /// <summary>Helper for getting <see cref="RootElementFinderProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="RootElementFinderProperty"/> from.</param>
        /// <remarks>Gets the root element finder.</remarks>
        /// <returns>RootElementFinder property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IRootElementFinder GetRootElementFinder(DependencyObject element)
        {
            return (IRootElementFinder)element.GetValue(RootElementFinderProperty);
        }

        /// <summary>Helper for setting <see cref="RootElementFinderProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="RootElementFinderProperty"/> on.</param>
        /// <param name="value">RootElementFinder property value.</param>
        /// <remarks>Sets the root element finder.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetRootElementFinder(DependencyObject element, IRootElementFinder value)
        {
            element.SetValue(RootElementFinderProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum items count which will be used for the dragged preview.
        /// </summary>
        public static readonly DependencyProperty DragPreviewMaxItemsCountProperty
            = DependencyProperty.RegisterAttached("DragPreviewMaxItemsCount",
                                                  typeof(int),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(10, null, (_, baseValue) =>
                                                      {
                                                          var itemsCount = (int)baseValue;
                                                          // Checking for MaxValue is maybe not necessary
                                                          return itemsCount < 0 ? 0 : itemsCount >= int.MaxValue ? int.MaxValue : itemsCount;
                                                      }));

        /// <summary>Helper for getting <see cref="DragPreviewMaxItemsCountProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragPreviewMaxItemsCountProperty"/> from.</param>
        /// <remarks>Gets the maximum items count which will be used for the dragged preview.</remarks>
        /// <returns>DragPreviewMaxItemsCount property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static int GetDragPreviewMaxItemsCount(DependencyObject element)
        {
            return (int)element.GetValue(DragPreviewMaxItemsCountProperty);
        }

        /// <summary>Helper for setting <see cref="DragPreviewMaxItemsCountProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragPreviewMaxItemsCountProperty"/> on.</param>
        /// <param name="value">DragPreviewMaxItemsCount property value.</param>
        /// <remarks>Sets the maximum items count which will be used for the dragged preview.</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragPreviewMaxItemsCount(DependencyObject element, int value)
        {
            element.SetValue(DragPreviewMaxItemsCountProperty, value);
        }

        /// <summary>
        /// Gets or sets the handler for the dragged preview items sorter
        /// </summary>
        public static readonly DependencyProperty DragPreviewItemsSorterProperty
            = DependencyProperty.RegisterAttached("DragPreviewItemsSorter",
                                                  typeof(IDragPreviewItemsSorter),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(null));

        /// <summary>Helper for getting <see cref="DragPreviewItemsSorterProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DragPreviewItemsSorterProperty"/> from.</param>
        /// <remarks>Gets the drag preview items sorter handler</remarks>
        /// <returns>DragPreviewItemsSorter property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDragPreviewItemsSorter GetDragPreviewItemsSorter(DependencyObject element)
        {
            return (IDragPreviewItemsSorter)element.GetValue(DragPreviewItemsSorterProperty);
        }

        /// <summary>Helper for setting <see cref="DragPreviewItemsSorterProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DragPreviewItemsSorterProperty"/> on.</param>
        /// <param name="value">DragPreviewItemsSorter property value.</param>
        /// <remarks>Sets the handler for the drag preview items sorter</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDragPreviewItemsSorter(DependencyObject element, IDragPreviewItemsSorter value)
        {
            element.SetValue(DragPreviewItemsSorterProperty, value);
        }

        /// <summary>
        /// Gets or sets the handler for the drop target items sorter
        /// </summary>
        public static readonly DependencyProperty DropTargetItemsSorterProperty
            = DependencyProperty.RegisterAttached("DropTargetItemsSorter",
                                                  typeof(IDropTargetItemsSorter),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(null));

        /// <summary>Helper for getting <see cref="DropTargetItemsSorterProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DropTargetItemsSorterProperty"/> from.</param>
        /// <remarks>Gets the drop target items sorter handler</remarks>
        /// <returns>DropTargetItemsSorter property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static IDropTargetItemsSorter GetDropTargetItemsSorter(DependencyObject element)
        {
            return (IDropTargetItemsSorter)element.GetValue(DropTargetItemsSorterProperty);
        }

        /// <summary>Helper for setting <see cref="DropTargetItemsSorterProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DropTargetItemsSorterProperty"/> on.</param>
        /// <param name="value">DropTargetItemsSorter property value.</param>
        /// <remarks>Sets the handler for the drop target items sorter</remarks>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetDropTargetItemsSorter(DependencyObject element, IDropTargetItemsSorter value)
        {
            element.SetValue(DropTargetItemsSorterProperty, value);
        }
    }
}