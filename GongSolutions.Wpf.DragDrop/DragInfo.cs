using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Holds information about a the source of a drag drop operation.
  /// </summary>
  /// 
  /// <remarks>
  /// The <see cref="DragInfo"/> class holds all of the framework's information about the source
  /// of a drag. It is used by <see cref="IDragSource.StartDrag"/> to determine whether a drag 
  /// can start, and what the dragged data should be.
  /// </remarks>
  public class DragInfo : IDragInfo
  {
    /// <summary>
    /// Initializes a new instance of the DragInfo class.
    /// </summary>
    /// 
    /// <param name="sender">
    /// The sender of the mouse event that initiated the drag.
    /// </param>
    /// 
    /// <param name="e">
    /// The mouse event that initiated the drag.
    /// </param>
    public DragInfo(object sender, MouseButtonEventArgs e)
    {
      this.DragStartPosition = e.GetPosition((IInputElement)sender);
      this.Effects = DragDropEffects.None;
      this.MouseButton = e.ChangedButton;
      this.VisualSource = sender as UIElement;
      this.DragDropCopyKeyState = DragDrop.GetDragDropCopyKeyState(this.VisualSource);

      if (sender is ItemsControl) {
        var itemsControl = (ItemsControl)sender;

        this.SourceGroup = itemsControl.FindGroup(this.DragStartPosition);
        this.VisualSourceFlowDirection = itemsControl.GetItemsPanelFlowDirection();

        var sourceItem = e.OriginalSource as UIElement; // If we can't cast object as a UIElement it might be a FrameworkContentElement, if so try and use its parent.
        if (sourceItem == null && e.OriginalSource is FrameworkContentElement) {
          sourceItem = ((FrameworkContentElement)e.OriginalSource).Parent as UIElement;
        }
        UIElement item = null;
        if (sourceItem != null) {
          item = itemsControl.GetItemContainer(sourceItem);
        }
          
        if (item == null) {
          if (DragDrop.GetDragDirectlySelectedOnly(VisualSource))
            item = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl));
          else
            item = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl), itemsControl.GetItemsPanelOrientation());
        }

        if (item != null) {
          // Remember the relative position of the item being dragged
          this.PositionInDraggedItem = e.GetPosition(item);

          var itemParent = ItemsControl.ItemsControlFromItemContainer(item);

          if (itemParent != null) {
            this.SourceCollection = itemParent.ItemsSource ?? itemParent.Items;
            this.SourceIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
            this.SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
          } else {
            this.SourceIndex = -1;
          }
          this.SourceItems = itemsControl.GetSelectedItems();

          // Some controls (I'm looking at you TreeView!) haven't updated their
          // SelectedItem by this point. Check to see if there 1 or less item in 
          // the SourceItems collection, and if so, override the control's 
          // SelectedItems with the clicked item.
          if (this.SourceItems.Cast<object>().Count() <= 1) {
            this.SourceItems = Enumerable.Repeat(this.SourceItem, 1);
          }

          this.VisualSourceItem = item;
        } else {
          this.SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
        }
      } else {
        if (sender is UIElement) {
          this.PositionInDraggedItem = e.GetPosition((UIElement)sender);
        }
      }

      if (this.SourceItems == null) {
        this.SourceItems = Enumerable.Empty<object>();
      }
    }

    /// <summary>
    /// Gets or sets the drag data.
    /// </summary>
    /// 
    /// <remarks>
    /// This must be set by a drag handler in order for a drag to start.
    /// </remarks>
    public object Data { get; set; }

    /// <summary>
    /// Gets the position of the click that initiated the drag, relative to <see cref="VisualSource"/>.
    /// </summary>
    public Point DragStartPosition { get; private set; }

    /// <summary>
    /// Gets the point where the cursor was relative to the item being dragged when the drag was started.
    /// </summary>
    public Point PositionInDraggedItem { get; private set; }

    /// <summary>
    /// Gets or sets the allowed effects for the drag.
    /// </summary>
    /// 
    /// <remarks>
    /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drag handler in order 
    /// for a drag to start.
    /// </remarks>
    public DragDropEffects Effects { get; set; }

    /// <summary>
    /// Gets the mouse button that initiated the drag.
    /// </summary>
    public MouseButton MouseButton { get; private set; }

    /// <summary>
    /// Gets the collection that the source ItemsControl is bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    public IEnumerable SourceCollection { get; private set; }

    /// <summary>
    /// Gets the position from where the item was dragged.
    /// </summary>
    /// <value>The index of the source.</value>
    public int SourceIndex { get; private set; }

    /// <summary>
    /// Gets the object that a dragged item is bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    public object SourceItem { get; private set; }

    /// <summary>
    /// Gets a collection of objects that the selected items in an ItemsControl are bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the control that initated the drag is unbound or not an ItemsControl, this will be empty.
    /// </remarks>
    public IEnumerable SourceItems { get; private set; }

    /// <summary>
    /// Gets the group from a dragged item if the drag is currently from an ItemsControl with groups.
    /// </summary>
    public CollectionViewGroup SourceGroup { get; private set; }

    /// <summary>
    /// Gets the control that initiated the drag.
    /// </summary>
    public UIElement VisualSource { get; private set; }

    /// <summary>
    /// Gets the item in an ItemsControl that started the drag.
    /// </summary>
    /// 
    /// <remarks>
    /// If the control that initiated the drag is an ItemsControl, this property will hold the item
    /// container of the clicked item. For example, if <see cref="VisualSource"/> is a ListBox this
    /// will hold a ListBoxItem.
    /// </remarks>
    public UIElement VisualSourceItem { get; private set; }

    /// <summary>
    /// Gets the FlowDirection of the current drag source.
    /// </summary>
    public FlowDirection VisualSourceFlowDirection { get; private set; }

    /// <summary>
    /// Gets the <see cref="IDataObject"/> which is used by the drag and drop operation. Set it to
    /// a custom instance if custom drag and drop behavior is needed.
    /// </summary>
    public IDataObject DataObject { get; set; }

    /// <summary>
    /// Gets the drag drop copy key state indicating the effect of the drag drop operation.
    /// </summary>
    public DragDropKeyStates DragDropCopyKeyState { get; private set; }
  }
}