using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Controls;
using System.Windows.Input;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// A default insertion drop handler for the most common usages
  /// </summary>
  public class DefaultDropHandler : IDropTarget
  {
    /// <summary>
    /// Updates the current drag state.
    /// </summary>
    /// <param name="dropInfo">Information about the drag.</param>
    /// <remarks>
    /// To allow a drop at the current drag position, the <see cref="DropInfo.Effects" /> property on
    /// <paramref name="dropInfo" /> should be set to a value other than <see cref="DragDropEffects.None" />
    /// and <see cref="DropInfo.Data" /> should be set to a non-null value.
    /// </remarks>
    public virtual void DragOver(IDropInfo dropInfo)
    {
      if (CanAcceptData(dropInfo)) {
        // default should always the move action/effect
        var copyData = (dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState)
                       //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                       && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
        dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
        var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
        dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
      }
    }

    /// <summary>
    /// Performs a drop.
    /// </summary>
    /// <param name="dropInfo">Information about the drop.</param>
    public virtual void Drop(IDropInfo dropInfo)
    {
      if (dropInfo == null || dropInfo.DragInfo == null) {
        return;
      }
      
      var insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;

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
      var data = ExtractData(dropInfo.Data);

      // default should always the move action/effect
      var copyData = (dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState)
                     //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                     && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                     && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                     && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
      if (!copyData)
      {
        var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();

        foreach (var o in data) {
          var index = sourceList.IndexOf(o);

          if (index != -1) {
            sourceList.RemoveAt(index);
            // so, is the source list the destination list too ?
            if (Equals(sourceList, destinationList) && index < insertIndex) {
              --insertIndex;
            }
          }
        }
      }

      var tabControl = dropInfo.VisualTarget as TabControl;

      // check for cloning
      var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy)
                      || dropInfo.Effects.HasFlag(DragDropEffects.Link);
      foreach (var o in data) {
        var obj2Insert = o;
        if (cloneData) {
          var cloneable = o as ICloneable;
          if (cloneable != null) {
            obj2Insert = cloneable.Clone();
          }
        }

        destinationList.Insert(insertIndex++, obj2Insert);

        if (tabControl != null) {
          // call ApplyTemplate for TabItem in TabControl to avoid this error:
          //
          // System.Windows.Data Error: 4 : Cannot find source for binding with reference
          // 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.TabControl', AncestorLevel='1''.
          // BindingExpression:Path=TabStripPlacement; DataItem=null; target element is 'TabItem' (Name='');
          // target property is 'NoTarget' (type 'Object')
          var container = tabControl.ItemContainerGenerator.ContainerFromItem(obj2Insert) as TabItem;
          if (container != null) {
            container.ApplyTemplate();
          }

          // for better experience: select the dragged TabItem
          tabControl.SetSelectedItem(obj2Insert);
        }
      }
    }

    /// <summary>
    /// Test the specified drop information for the right data.
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public static bool CanAcceptData(IDropInfo dropInfo)
    {
      if (dropInfo == null || dropInfo.DragInfo == null) {
        return false;
      }

      if (!dropInfo.IsSameDragDropContextAsSource) {
        return false;
      }

      // do not drop on itself
      var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
                           && dropInfo.VisualTargetItem is TreeViewItem;
      if (isTreeViewItem && dropInfo.VisualTargetItem == dropInfo.DragInfo.VisualSourceItem) {
        return false;
      }

      if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection) {
        var targetList = dropInfo.TargetCollection.TryGetList();
        return targetList != null;
      }
//      else if (dropInfo.DragInfo.SourceCollection is ItemCollection) {
//        return false;
//      }
      else if (dropInfo.TargetCollection == null) {
        return false;
      } else {
        if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data)) {
          var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
          return !isChildOf;
        } else {
          return false;
        }
      }
    }

    public static IEnumerable ExtractData(object data)
    {
      if (data is IEnumerable && !(data is string)) {
        return (IEnumerable)data;
      } else {
        return Enumerable.Repeat(data, 1);
      }
    }

    protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
    {
      var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

      while (parent != null) {
        if (parent == sourceItem) {
          return true;
        }

        parent = ItemsControl.ItemsControlFromItemContainer(parent);
      }

      return false;
    }

    protected static bool TestCompatibleTypes(IEnumerable target, object data)
    {
      TypeFilter filter = (t, o) => {
                            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                          };

      var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
      var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

      if (enumerableTypes.Count() > 0) {
        var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
        return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
      } else {
        return target is IList;
      }
    }
  }
}