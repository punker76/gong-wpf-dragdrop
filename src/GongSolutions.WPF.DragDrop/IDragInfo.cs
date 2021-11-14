using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GongSolutions.Wpf.DragDrop
{
    public interface IDragInfo
    {
        /// <summary>
        /// Gets or sets the data format which will be used for the drag and drop actions.
        /// </summary>
        /// <value>The data format.</value>
        DataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        /// <remarks>
        /// This must be set by a drag handler in order for a drag to start.
        /// </remarks>
        object Data { get; set; }

        /// <summary>
        /// Gets the position of the click that initiated the drag, relative to <see cref="VisualSource"/>.
        /// </summary>
        Point DragStartPosition { get; }

        /// <summary>
        /// Gets the point where the cursor was relative to the item being dragged when the drag was started.
        /// </summary>
        Point PositionInDraggedItem { get; }

        /// <summary>
        /// Gets or sets the allowed effects for the drag.
        /// </summary>
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drag handler in order 
        /// for a drag to start.
        /// </remarks>
        DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the mouse button that initiated the drag.
        /// </summary>
        MouseButton MouseButton { get; }

        /// <summary>
        /// Gets the collection that the source ItemsControl is bound to.
        /// </summary>
        /// <remarks>
        /// If the control that initiated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        IEnumerable SourceCollection { get; }

        /// <summary>
        /// Gets the position from where the item was dragged.
        /// </summary>
        int SourceIndex { get; }

        /// <summary>
        /// Gets the object that a dragged item is bound to.
        /// </summary>
        /// <remarks>
        /// If the control that initiated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        object SourceItem { get; }

        /// <summary>
        /// Gets a collection of objects that the selected items in an ItemsControl are bound to.
        /// </summary>
        /// <remarks>
        /// If the control that initiated the drag is unbound or not an ItemsControl, this will be empty.
        /// </remarks>
        IEnumerable SourceItems { get; }

        /// <summary>
        /// Gets the group from a dragged item if the drag is currently from an ItemsControl with groups.
        /// </summary>
        CollectionViewGroup SourceGroup { get; }

        /// <summary>
        /// Gets the control that initiated the drag.
        /// </summary>
        UIElement VisualSource { get; }

        /// <summary>
        /// Gets the item in an ItemsControl that started the drag.
        /// </summary>
        /// <remarks>
        /// If the control that initiated the drag is an ItemsControl, this property will hold the item
        /// container of the clicked item. For example, if <see cref="VisualSource"/> is a ListBox this
        /// will hold a ListBoxItem.
        /// </remarks>
        UIElement VisualSourceItem { get; }

        /// <summary>
        /// Gets the FlowDirection of the current drag source.
        /// </summary>
        FlowDirection VisualSourceFlowDirection { get; }

        /// <summary>
        /// Gets the <see cref="IDataObject"/> which is used by the drag and drop operation. Set it to
        /// a custom instance if custom drag and drop behavior is needed.
        /// </summary>
        object DataObject { get; set; }

#pragma warning disable 1572
        /// <summary>Initiates a drag-and-drop operation.</summary>
        /// <param name="dragSource">A reference to the dependency object that is the source of the data being dragged.</param>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the <see cref="T:System.Windows.DragDropEffects" /> values that specifies permitted effects of the drag-and-drop operation.</param>
        /// <returns>One of the <see cref="T:System.Windows.DragDropEffects" /> values that specifies the final effect that was performed during the drag-and-drop operation.</returns>
#pragma warning restore 1572
        Func<DependencyObject, object, DragDropEffects, DragDropEffects> DragDropHandler { get; set; }

        /// <summary>
        /// Gets the drag drop copy key state indicating the effect of the drag drop operation.
        /// </summary>
        DragDropKeyStates DragDropCopyKeyState { get; }
    }
}