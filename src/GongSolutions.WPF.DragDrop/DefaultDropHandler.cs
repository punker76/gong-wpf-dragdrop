using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;
using JetBrains.Annotations;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// A default insertion drop handler for the most common usages
    /// </summary>
    public class DefaultDropHandler : IDropTargetHint
    {
        /// <summary>
        /// Test the specified drop information for the right data.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return false;
            }

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                return false;
            }

            // do not drop on itself
            var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
                                 && dropInfo.VisualTargetItem is TreeViewItem;
            if (isTreeViewItem && dropInfo.VisualTargetItem == dropInfo.DragInfo.VisualSourceItem)
            {
                return false;
            }

            if (dropInfo.TargetCollection is null)
            {
                return false;
            }

            if (ReferenceEquals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection))
            {
                var targetList = dropInfo.TargetCollection.TryGetList();
                return targetList != null;
            }

            if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
            {
                var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                return !isChildOf;
            }

            return false;
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable enumerable and not string)
            {
                return enumerable;
            }

            return Enumerable.Repeat(data, 1);
        }

        /// <summary>
        /// Clears the current selected items and selects the given items.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        /// <param name="items">The items which should be select.</param>
        /// <param name="applyTemplate">if set to <c>true</c> then for all items the ApplyTemplate will be invoked.</param>
        /// <param name="focusVisualTarget">if set to <c>true</c> the visual target will be focused.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="dropInfo" /> is <see langword="null" /></exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="dropInfo" /> is <see langword="null" /></exception>
        public static void SelectDroppedItems([NotNull] IDropInfo dropInfo, [NotNull] IEnumerable items, bool applyTemplate = true, bool focusVisualTarget = true)
        {
            if (dropInfo == null) throw new ArgumentNullException(nameof(dropInfo));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                var tvItem = dropInfo.VisualTargetItem as TreeViewItem;
                var tvItemIsExpanded = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;

                var itemsParent = tvItemIsExpanded
                    ? tvItem
                    : dropInfo.VisualTargetItem != null
                        ? ItemsControl.ItemsControlFromItemContainer(dropInfo.VisualTargetItem)
                        : itemsControl;
                itemsParent ??= itemsControl;

                (dropInfo.DragInfo.VisualSourceItem as TreeViewItem)?.ClearSelectedItems();
                itemsParent.ClearSelectedItems();

                var selectDroppedItems = dropInfo.VisualTarget is TabControl || (dropInfo.VisualTarget != null && DragDrop.GetSelectDroppedItems(dropInfo.VisualTarget));
                if (selectDroppedItems)
                {
                    foreach (var item in items)
                    {
                        if (applyTemplate)
                        {
                            // call ApplyTemplate for TabItem in TabControl to avoid this error:
                            //
                            // System.Windows.Data Error: 4 : Cannot find source for binding with reference
                            var container = itemsParent.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                            container?.ApplyTemplate();
                        }

                        itemsParent.SetItemSelected(item, true);
                    }

                    if (focusVisualTarget)
                    {
                        itemsControl.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the data of the drag drop action should be copied otherwise moved.
        /// </summary>
        /// <param name="dropInfo">The DropInfo with a valid DragInfo.</param>
        public static bool ShouldCopyData(IDropInfo dropInfo)
        {
            // default should always the move action/effect
            if (dropInfo?.DragInfo == null)
            {
                return false;
            }

            var copyData = ((dropInfo.DragInfo.DragDropCopyKeyState != default) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState))
                           || dropInfo.DragInfo.DragDropCopyKeyState.HasFlag(DragDropKeyStates.LeftMouseButton);
            copyData = copyData
                       && dropInfo.DragInfo.SourceItem is not HeaderedContentControl
                       && dropInfo.DragInfo.SourceItem is not HeaderedItemsControl
                       && dropInfo.DragInfo.SourceItem is not ListBoxItem;
            return copyData;
        }

        protected static int GetInsertIndex(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.UnfilteredInsertIndex;

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                if (itemsControl.Items is IEditableCollectionView editableItems)
                {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0)
                    {
                        ++insertIndex;
                    }
                    else if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count)
                    {
                        --insertIndex;
                    }
                }
            }

            return insertIndex;
        }

        protected static void Move(IList list, int sourceIndex, int destinationIndex)
        {
            if (!list.IsObservableCollection())
            {
                throw new ArgumentException("ObservableCollection<T> was expected", nameof(list));
            }

            if (sourceIndex != destinationIndex)
            {
                var method = list.GetType().GetMethod("Move", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                _ = method?.Invoke(list, new object[] { sourceIndex, destinationIndex });
            }
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            if (data == null)
            {
                return false;
            }

            bool InterfaceFilter(Type t, object o) => (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            var enumerableInterfaces = target.GetType().FindInterfaces(InterfaceFilter, null);
            var enumerableTypes = from i in enumerableInterfaces
                                  select i.GetGenericArguments().Single();

            if (enumerableTypes.Any())
            {
                var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList or ICollectionView;
            }
        }

        public virtual void DropHint(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Hint;
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragEnter(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                var copyData = ShouldCopyData(dropInfo);
                dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragLeave(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public virtual void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return;
            }

            var insertIndex = GetInsertIndex(dropInfo);
            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool isSameCollection = false;

            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (sourceList != null)
                {
                    isSameCollection = sourceList.IsSameObservableCollection(destinationList);
                    if (!isSameCollection)
                    {
                        foreach (var o in data)
                        {
                            var index = sourceList.IndexOf(o);
                            if (index != -1)
                            {
                                sourceList.RemoveAt(index);

                                // If source is destination too fix the insertion index
                                if (destinationList != null && ReferenceEquals(sourceList, destinationList) && index < insertIndex)
                                {
                                    --insertIndex;
                                }
                            }
                        }
                    }
                }
            }

            if (destinationList != null)
            {
                var objects2Insert = new List<object>();

                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy) || dropInfo.Effects.HasFlag(DragDropEffects.Link);

                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        if (o is ICloneableDragItem cloneableItem)
                        {
                            obj2Insert = cloneableItem.CloneItem(dropInfo);
                        }
                        else if (o is ICloneable cloneable)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);

                    if (!cloneData && isSameCollection)
                    {
                        var index = destinationList.IndexOf(o);
                        if (index != -1)
                        {
                            if (insertIndex > index)
                            {
                                insertIndex--;
                            }

                            Move(destinationList, index, insertIndex++);
                        }
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }

                    if (obj2Insert is IDragItemSource dragItemSource)
                    {
                        dragItemSource.ItemDropped(dropInfo);
                    }
                }

                SelectDroppedItems(dropInfo, objects2Insert);
            }
        }
    }
}