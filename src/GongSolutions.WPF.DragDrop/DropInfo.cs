using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GongSolutions.Wpf.DragDrop.Utilities;
using JetBrains.Annotations;

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
        private readonly DragEventArgs eventArgs;
        private ItemsControl itemParent;
        private bool? acceptChildItem;

        /// <inheritdoc />
        public object Data { get; set; }

        /// <inheritdoc />
        public IDragInfo DragInfo { get; protected set; }

        /// <inheritdoc />
        public Point DropPosition { get; protected set; }

        /// <inheritdoc />
        public Type DropTargetAdorner { get; set; }

        /// <inheritdoc />
        public DragDropEffects Effects { get; set; }

        /// <inheritdoc />
        public int InsertIndex { get; protected set; }

        /// <inheritdoc />
        public int UnfilteredInsertIndex
        {
            get
            {
                var insertIndex = this.InsertIndex;
                if (this.itemParent is null)
                {
                    return insertIndex;
                }

                var itemSourceAsList = this.itemParent.ItemsSource.TryGetList();
                if (itemSourceAsList != null && this.itemParent.Items != null && this.itemParent.Items.Count != itemSourceAsList.Count)
                {
                    if (insertIndex >= 0 && insertIndex < this.itemParent.Items.Count)
                    {
                        var indexOf = itemSourceAsList.IndexOf(this.itemParent.Items[insertIndex]);
                        if (indexOf >= 0)
                        {
                            return indexOf;
                        }
                    }
                    else if (this.itemParent.Items.Count > 0 && insertIndex == this.itemParent.Items.Count)
                    {
                        var indexOf = itemSourceAsList.IndexOf(this.itemParent.Items[insertIndex - 1]);
                        if (indexOf >= 0)
                        {
                            return indexOf + 1;
                        }
                    }
                }

                return insertIndex;
            }
        }

        /// <inheritdoc />
        public IEnumerable TargetCollection { get; protected set; }

        /// <inheritdoc />
        public object TargetItem { get; protected set; }

        /// <inheritdoc />
        public CollectionViewGroup TargetGroup { get; protected set; }

        /// <summary>
        /// Gets the ScrollViewer control for the visual target.
        /// </summary>
        public ScrollViewer TargetScrollViewer { get; protected set; }

        /// <summary>
        /// Gets or Sets the ScrollingMode for the drop action.
        /// </summary>
        public ScrollingMode TargetScrollingMode { get; set; }

        /// <inheritdoc />
        public UIElement VisualTarget { get; protected set; }

        /// <inheritdoc />
        public UIElement VisualTargetItem { get; protected set; }

        /// <inheritdoc />
        public Orientation VisualTargetOrientation { get; protected set; }

        /// <inheritdoc />
        public FlowDirection VisualTargetFlowDirection { get; protected set; }

        /// <inheritdoc />
        public string DestinationText { get; set; }

        /// <inheritdoc />
        public string EffectText { get; set; }

        /// <inheritdoc />
        public RelativeInsertPosition InsertPosition { get; protected set; }

        /// <inheritdoc />
        public DragDropKeyStates KeyStates { get; protected set; }

        /// <inheritdoc />
        public bool NotHandled { get; set; }

        /// <inheritdoc />
        public bool IsSameDragDropContextAsSource
        {
            get
            {
                // Check if DragInfo stuff exists
                if (this.DragInfo?.VisualSource is null)
                {
                    return true;
                }

                // A target should be exists
                if (this.VisualTarget is null)
                {
                    return true;
                }

                // Source element has a drag context constraint, we need to check the target property matches.
                var sourceContext = DragDrop.GetDragDropContext(this.DragInfo.VisualSource);
                var targetContext = DragDrop.GetDragDropContext(this.VisualTarget);

                return string.Equals(sourceContext, targetContext)
                       || string.IsNullOrEmpty(targetContext);
            }
        }

        /// <inheritdoc />
        public EventType EventType { get; }

        /// <summary>
        /// Indicates if the drop target can accept the dragged data as a child item (applies to tree view items).
        /// </summary>
        /// <remarks>
        /// Changing this value will update other properties.
        /// </remarks>
        public bool AcceptChildItem
        {
            get => this.acceptChildItem.GetValueOrDefault();
            set
            {
                if (value != this.acceptChildItem)
                {
                    this.acceptChildItem = value;
                    Update();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// <param name="sender">The sender of the drop event.</param>
        /// <param name="e">The drag event arguments.</param>
        /// <param name="dragInfo">Information about the drag source, if the drag came from within the framework.</param>
        /// <param name="eventType">The type of the underlying event (tunneled or bubbled).</param>
        public DropInfo(object sender, DragEventArgs e, [CanBeNull] IDragInfo dragInfo, EventType eventType)
        {
            this.eventArgs = e;
            this.DragInfo = dragInfo;
            this.KeyStates = e.KeyStates;
            this.EventType = eventType;
            var dataFormat = dragInfo?.DataFormat;
            this.Data = dataFormat != null && e.Data.GetDataPresent(dataFormat.Name) ? e.Data.GetData(dataFormat.Name) : e.Data;

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
            var dropTargetScrollViewer = DragDrop.GetDropTargetScrollViewer(this.VisualTarget);
            if (dropTargetScrollViewer != null)
            {
                this.TargetScrollViewer = dropTargetScrollViewer;
            }
            else if (this.VisualTarget is TabControl)
            {
                var tabPanel = this.VisualTarget.GetVisualDescendent<TabPanel>();
                this.TargetScrollViewer = tabPanel?.GetVisualAncestor<ScrollViewer>();
            }
            else
            {
                this.TargetScrollViewer = this.VisualTarget?.GetVisualDescendent<ScrollViewer>();
            }

            this.TargetScrollingMode = this.VisualTarget != null ? DragDrop.GetDropScrollingMode(this.VisualTarget) : ScrollingMode.Both;

            // visual target can be null, so give us a point...
            this.DropPosition = this.VisualTarget != null ? e.GetPosition(this.VisualTarget) : new Point();

            Update();
        }

        private void Update()
        {
            if (this.VisualTarget is TabControl)
            {
                if (!HitTestUtilities.HitTest4Type<TabPanel>(this.VisualTarget, this.DropPosition))
                {
                    return;
                }
            }

            if (this.VisualTarget is ItemsControl itemsControl)
            {
                //System.Diagnostics.Debug.WriteLine(">>> Name = {0}", itemsControl.Name);

                // get item under the mouse
                var item = itemsControl.GetItemContainerAt(this.DropPosition);
                var directlyOverItem = item != null;

                this.TargetGroup = itemsControl.FindGroup(this.DropPosition);
                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                this.VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (item == null)
                {
                    // ok, no item found, so maybe we can found an item at top, left, right or bottom
                    item = itemsControl.GetItemContainerAt(this.DropPosition, this.VisualTargetOrientation);
                    directlyOverItem = this.DropPosition.DirectlyOverElement(item, itemsControl);
                }

                if (item == null && this.TargetGroup is { IsBottomLevel: true })
                {
                    var itemData = this.TargetGroup.Items.FirstOrDefault();
                    if (itemData != null)
                    {
                        item = itemsControl.ItemContainerGenerator.ContainerFromItem(itemData) as UIElement;
                        directlyOverItem = this.DropPosition.DirectlyOverElement(item, itemsControl);
                    }
                }

                if (item != null)
                {
                    this.itemParent = ItemsControl.ItemsControlFromItemContainer(item);
                    this.VisualTargetOrientation = this.itemParent.GetItemsPanelOrientation();
                    this.VisualTargetFlowDirection = this.itemParent.GetItemsPanelFlowDirection();

                    this.InsertIndex = this.itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    this.TargetCollection = this.itemParent.ItemsSource ?? this.itemParent.Items;

                    var tvItem = item as TreeViewItem;

                    if (directlyOverItem || tvItem != null)
                    {
                        this.VisualTargetItem = item;
                        this.TargetItem = this.itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    }

                    var tvItemIsExpanded = tvItem is { HasHeader: true, HasItems: true, IsExpanded: true };
                    var itemRenderSize = tvItemIsExpanded ? tvItem.GetHeaderSize() : item.RenderSize;
                    this.acceptChildItem ??= tvItem != null;

                    if (this.VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = this.eventArgs.GetPosition(item).Y;
                        var targetHeight = itemRenderSize.Height;

                        var topGap = targetHeight * 0.25;
                        var bottomGap = targetHeight * 0.75;
                        if (currentYPos > targetHeight / 2)
                        {
                            if (tvItemIsExpanded && (currentYPos < topGap || currentYPos > bottomGap))
                            {
                                this.VisualTargetItem = tvItem.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                                this.TargetItem = this.VisualTargetItem != null ? tvItem.ItemContainerGenerator.ItemFromContainer(this.VisualTargetItem) : null;
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = 0;
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else
                        {
                            this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (this.AcceptChildItem && currentYPos > topGap && currentYPos < bottomGap)
                        {
                            if (tvItem != null)
                            {
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
                            }

                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }

                        // System.Diagnostics.Debug.WriteLine($"==> DropInfo: pos={this.InsertPosition}, index={this.InsertIndex}, currentXPos={currentXPos}, Item={this.item}");
                    }
                    else
                    {
                        var currentXPos = this.eventArgs.GetPosition(item).X;
                        var targetWidth = itemRenderSize.Width;

                        if (this.VisualTargetFlowDirection == FlowDirection.RightToLeft)
                        {
                            if (currentXPos < targetWidth / 2)
                            {
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else if (this.VisualTargetFlowDirection == FlowDirection.LeftToRight)
                        {
                            if (currentXPos > targetWidth / 2)
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                            else
                            {
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                        }

                        if (this.AcceptChildItem && currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75)
                        {
                            if (tvItem != null)
                            {
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
                            }

                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }

                        // System.Diagnostics.Debug.WriteLine($"==> DropInfo: pos={this.InsertPosition}, index={this.InsertIndex}, currentXPos={currentXPos}, Item={this.item}");
                    }
                }
                else
                {
                    this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    this.InsertIndex = itemsControl.Items.Count;
                    //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, item=NULL, idx={1}", this.InsertPosition, this.InsertIndex);
                }
            }
            else
            {
                this.VisualTargetItem = this.VisualTarget;
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