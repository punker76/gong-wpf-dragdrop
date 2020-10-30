﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DefaultDropHandler : IDropTarget
    {
        /// <summary>
        /// Test the specified drop information for the right data.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
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

            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                var targetList = dropInfo.TargetCollection.TryGetList();
                return targetList != null;
            }
            //      else if (dropInfo.DragInfo.SourceCollection is ItemCollection) {
            //        return false;
            //      }
            else if (dropInfo.TargetCollection == null)
            {
                return false;
            }
            else
            {
                if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                {
                    var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                    return !isChildOf;
                }
                else
                {
                    return false;
                }
            }
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
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
            var itemsControl = dropInfo.VisualTarget as ItemsControl;
            if (itemsControl != null)
            {
                var tvItem = dropInfo.VisualTargetItem as TreeViewItem;
                var tvItemIsExpanded = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;

                var itemsParent = tvItemIsExpanded ? tvItem : (dropInfo.VisualTargetItem != null ? ItemsControl.ItemsControlFromItemContainer(dropInfo.VisualTargetItem) : itemsControl);
                itemsParent = itemsParent ?? itemsControl;

                itemsParent.ClearSelectedItems();

                foreach (var obj in items)
                {
                    if (applyTemplate)
                    {
                        // call ApplyTemplate for TabItem in TabControl to avoid this error:
                        //
                        // System.Windows.Data Error: 4 : Cannot find source for binding with reference
                        var container = itemsParent.ItemContainerGenerator.ContainerFromItem(obj) as FrameworkElement;
                        container?.ApplyTemplate();
                    }
                    itemsParent.SetItemSelected(obj, true);
                }

                if (focusVisualTarget)
                {
                    itemsControl.Focus();
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
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return false;
            }
            var copyData = ((dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState))
                           || dropInfo.DragInfo.DragDropCopyKeyState.HasFlag(DragDropKeyStates.LeftMouseButton);
            copyData = copyData
                       //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                       && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
            return copyData;
        }

        /// <inheritdoc />
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = ShouldCopyData(dropInfo) ? DragDropEffects.Copy : DragDropEffects.Move;
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
            }
        }

        /// <inheritdoc />
        public virtual void Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return;
            }

            var insertIndex = dropInfo.UnfilteredInsertIndex;

            var itemsControl = dropInfo.VisualTarget as ItemsControl;
            if (itemsControl != null)
            {
                var editableItems = itemsControl.Items as IEditableCollectionView;
                if (editableItems != null)
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

            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool forceMoveBehavior = false;
            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (sourceList != null)
                {
                    forceMoveBehavior = SameObservableCollection(sourceList, destinationList);
                    if (!forceMoveBehavior)
                    {
                        foreach (var o in data)
                        {
                            var index = sourceList.IndexOf(o);
                            if (index != -1)
                            {
                                sourceList.RemoveAt(index);
                                // so, is the source list the destination list too ?
                                if (destinationList != null && Equals(sourceList, destinationList) && index < insertIndex)
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
                        var cloneable = o as ICloneable;
                        if (cloneable != null)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);
                    if (!cloneData && forceMoveBehavior)
                    {
                        int index = destinationList.IndexOf(o);
                        if (insertIndex > index)
                        {
                            insertIndex--;
                        }
                        Move(destinationList, index, insertIndex++);
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }
                }

                var selectDroppedItems = itemsControl is TabControl || (itemsControl != null && DragDrop.GetSelectDroppedItems(itemsControl));
                if (selectDroppedItems)
                {
                    SelectDroppedItems(dropInfo, objects2Insert);
                }
            }
        }

        private static void Move(IList list, int sourceIndex, int destinationIndex)
        {
            if (!IsObservableCollection(list))
                throw new ArgumentException("ObservableCollection<T> was expected", nameof(list));
            var method = list.GetType().GetMethod("Move",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public);            
            method.Invoke(list, new object[] { sourceIndex, destinationIndex });            
        }

        private static bool SameObservableCollection(IList collection1, IList collection2)
        {
            return ReferenceEquals(collection1, collection2) && IsObservableCollection(collection1);
        }

        private static bool IsObservableCollection(IList collection) 
        {
            return 
                collection.GetType().IsGenericType &&
                collection.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>);
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
                return target is IList || target is ICollectionView;
            }
        }
    }
}
