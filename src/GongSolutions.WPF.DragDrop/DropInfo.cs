using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Data;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Holds information about a the target of a drag drop operation.
  /// </summary>
  /// 
  /// <remarks>
  /// The <see cref="DropInfo"/> class holds all of the framework's information about the current 
  /// target of a drag. It is used by <see cref="IDropTarget.DragOver"/> method to determine whether 
  /// the current drop target is valid, and by <see cref="IDropTarget.Drop"/> to perform the drop.
  /// </remarks>
  public class DropInfo : IDropInfo
  {
    private ItemsControl itemParent = null;
    private UIElement item = null;
    
    /// <summary>
    /// Initializes a new instance of the DropInfo class.
    /// </summary>
    /// 
    /// <param name="sender">
    /// The sender of the drag event.
    /// </param>
    /// 
    /// <param name="e">
    /// The drag event.
    /// </param>
    /// 
    /// <param name="dragInfo">
    /// Information about the source of the drag, if the drag came from within the framework.
    /// </param>
    public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo)
    {
      var dataFormat = DragDrop.DataFormat.Name;
      this.Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
      this.DragInfo = dragInfo;
      this.KeyStates = e.KeyStates;

      this.VisualTarget = sender as UIElement;
      // if there is no drop target, find another
      if (!this.VisualTarget.IsDropTarget())
      {
        // try to find next element
        var element = this.VisualTarget.TryGetNextAncestorDropTargetElement();
        if (element != null)
        {
          this.VisualTarget = element;
        }
      }

      // try find ScrollViewer
      if (this.VisualTarget is TabControl)
      {
        var tabPanel = this.VisualTarget.GetVisualDescendent<TabPanel>();
        this.TargetScrollViewer = tabPanel?.GetVisualAncestor<ScrollViewer>();
      }
      else
      {
        this.TargetScrollViewer = this.VisualTarget?.GetVisualDescendent<ScrollViewer>();
      }

      // visual target can be null, so give us a point...
      this.DropPosition = this.VisualTarget != null ? e.GetPosition(this.VisualTarget) : new Point();

      if (this.VisualTarget is TabControl) {
        if (!HitTestUtilities.HitTest4Type<TabPanel>(this.VisualTarget, this.DropPosition)) {
          return;
        }
      }

      if (this.VisualTarget is ItemsControl) {
        var itemsControl = (ItemsControl)this.VisualTarget;
        //System.Diagnostics.Debug.WriteLine(">>> Name = {0}", itemsControl.Name);
        // get item under the mouse
        item = itemsControl.GetItemContainerAt(this.DropPosition);
        var directlyOverItem = item != null;

        this.TargetGroup = itemsControl.FindGroup(this.DropPosition);
        this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
        this.VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

        if (item == null) {
          // ok, no item found, so maybe we can found an item at top, left, right or bottom
          item = itemsControl.GetItemContainerAt(this.DropPosition, this.VisualTargetOrientation);
          directlyOverItem = false;
        }

        if (item == null && this.TargetGroup != null && this.TargetGroup.IsBottomLevel)
        {
          var itemData = this.TargetGroup.Items.FirstOrDefault();
          if (itemData != null)
          {
            item = itemsControl.ItemContainerGenerator.ContainerFromItem(itemData) as UIElement;
            directlyOverItem = false;
          }
        }

        if (item != null)
        {
          itemParent = ItemsControl.ItemsControlFromItemContainer(item);
          this.VisualTargetOrientation = itemParent.GetItemsPanelOrientation();
          this.VisualTargetFlowDirection = itemParent.GetItemsPanelFlowDirection();

          this.InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
          this.TargetCollection = itemParent.ItemsSource ?? itemParent.Items;

          var tvItem = item as TreeViewItem;
          
          if (directlyOverItem || tvItem != null)
          {
            this.TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
            this.VisualTargetItem = item;
          }

          var itemRenderSize = item.RenderSize;

          if (this.VisualTargetOrientation == Orientation.Vertical) {
            var currentYPos = e.GetPosition(item).Y;
            var targetHeight = itemRenderSize.Height;

            if (currentYPos > targetHeight / 2) {
              this.InsertIndex++;
              this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
            } else {
              this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
            }

            if (currentYPos > targetHeight * 0.25 && currentYPos < targetHeight * 0.75) {
              if (tvItem != null)
              {
                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
              }
              this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
            }
            //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, Y={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentYPos, item);
          }
          else {
            var currentXPos = e.GetPosition(item).X;
            var targetWidth = itemRenderSize.Width;

            if (this.VisualTargetFlowDirection == FlowDirection.RightToLeft) {
              if (currentXPos > targetWidth / 2) {
                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
              } else {
                this.InsertIndex++;
                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
              }
            } else if (this.VisualTargetFlowDirection == FlowDirection.LeftToRight) {
              if (currentXPos > targetWidth / 2) {
                this.InsertIndex++;
                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
              } else {
                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
              }
            }

            if (currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75) {
              if (tvItem != null)
              {
                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
              }
              this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
            }
            //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, X={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentXPos, item);
          }
        }
        else
        {
          this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
          this.InsertIndex = itemsControl.Items.Count;
          //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, item=NULL, idx={1}", this.InsertPosition, this.InsertIndex);
        }
      } else {
          this.VisualTargetItem = this.VisualTarget; 
      }
    }

    /// <summary>
    /// Gets the drag data.
    /// </summary>
    /// 
    /// <remarks>
    /// If the drag came from within the framework, this will hold:
    /// 
    /// - The dragged data if a single item was dragged.
    /// - A typed IEnumerable if multiple items were dragged.
    /// </remarks>
    public object Data { get; private set; }

    /// <summary>
    /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
    /// if the drag came from within the framework.
    /// </summary>
    public IDragInfo DragInfo { get; private set; }

    /// <summary>
    /// Gets the mouse position relative to the VisualTarget
    /// </summary>
    public Point DropPosition { get; private set; }

    /// <summary>
    /// Gets or sets the class of drop target to display.
    /// </summary>
    /// 
    /// <remarks>
    /// The standard drop target adorner classes are held in the <see cref="DropTargetAdorners"/>
    /// class.
    /// </remarks>
    public Type DropTargetAdorner { get; set; }

    /// <summary>
    /// Gets or sets the allowed effects for the drop.
    /// </summary>
    /// 
    /// <remarks>
    /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler in order 
    /// for a drop to be possible.
    /// </remarks>
    public DragDropEffects Effects { get; set; }

    /// <summary>
    /// Gets the current insert position within <see cref="TargetCollection"/>.
    /// </summary>
    public int InsertIndex { get; private set; }

    /// <summary>
    /// Gets the current insert position within the source (unfiltered) <see cref="TargetCollection"/>.
    /// </summary>
    /// <remarks>
    /// This should be only used in a Drop action.
    /// This works only correct with different objects (string, int, etc won't work correct).
    /// </remarks>
    public int UnfilteredInsertIndex
    {
      get
      {
        var insertIndex = this.InsertIndex;
        if (itemParent != null) {
          var itemSourceAsList = itemParent.ItemsSource.TryGetList();
          if (itemSourceAsList != null && itemParent.Items != null && itemParent.Items.Count != itemSourceAsList.Count) {
            if (insertIndex >= 0 && insertIndex < itemParent.Items.Count) {
              var indexOf = itemSourceAsList.IndexOf(itemParent.Items[insertIndex]);
              if (indexOf >= 0) {
                return indexOf;
              }
            }
          }
        }
        return insertIndex;
      }
    }

    /// <summary>
    /// Gets the collection that the target ItemsControl is bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    public IEnumerable TargetCollection { get; private set; }

    /// <summary>
    /// Gets the object that the current drop target is bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    public object TargetItem { get; private set; }

    /// <summary>
    /// Gets the current group target.
    /// </summary>
    /// 
    /// <remarks>
    /// If the drag is currently over an ItemsControl with groups, describes the group that
    /// the drag is currently over.
    /// </remarks>
    public CollectionViewGroup TargetGroup { get; private set; }

    /// <summary>
    /// Gets the ScrollViewer control for the visual target.
    /// </summary>
    public ScrollViewer TargetScrollViewer { get; private set; }

    /// <summary>
    /// Gets the control that is the current drop target.
    /// </summary>
    public UIElement VisualTarget { get; private set; }

    /// <summary>
    /// Gets the item in an ItemsControl that is the current drop target.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    public UIElement VisualTargetItem { get; private set; }

    /// <summary>
    /// Gets the orientation of the current drop target.
    /// </summary>
    public Orientation VisualTargetOrientation { get; private set; }

    /// <summary>
    /// Gets the orientation of the current drop target.
    /// </summary>
    public FlowDirection VisualTargetFlowDirection { get; private set; }

    /// <summary>
    /// Gets and sets the text displayed in the DropDropEffects adorner.
    /// </summary>
    public string DestinationText { get; set; }

    /// <summary>
    /// Gets the relative position the item will be inserted to compared to the TargetItem
    /// </summary>
    public RelativeInsertPosition InsertPosition { get; private set; }

    /// <summary>
    /// Gets a flag enumeration indicating the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.
    /// </summary>
    public DragDropKeyStates KeyStates { get; private set; }

    public bool NotHandled { get; set; }

    /// <summary>
    /// Gets a value indicating whether the target is in the same context as the source, <see cref="DragDrop.DragDropContextProperty" />.
    /// </summary>
    public bool IsSameDragDropContextAsSource
    {
        get
        {
            // Check if DragInfo stuff exists
            if (this.DragInfo == null || this.DragInfo.VisualSource == null) {
                return true;
            }
            // A target should be exists
            if (this.VisualTarget == null) {
                return true;
            }

            // Source element has a drag context constraint, we need to check the target property matches.
            var sourceContext = this.DragInfo.VisualSource.GetValue(DragDrop.DragDropContextProperty) as string;
            if (String.IsNullOrEmpty(sourceContext)) {
                return true;
            }
            var targetContext = this.VisualTarget.GetValue(DragDrop.DragDropContextProperty) as string;
            return string.Equals(sourceContext, targetContext);
        }
    }
  }

  [Flags]
  public enum RelativeInsertPosition
  {
    None = 0,
    BeforeTargetItem = 1,
    AfterTargetItem = 2,
    TargetItemCenter = 4
  }
}
