﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Controls;
using System.ComponentModel;

namespace GongSolutions.Wpf.DragDrop
{
  public class DefaultDropHandler : IDropTarget
  {
    public virtual void DragOver(IDropInfo dropInfo)
    {
      if (CanAcceptData(dropInfo)) {
        dropInfo.Effects = DragDropEffects.Copy;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
      }
    }

    public virtual void Drop(IDropInfo dropInfo)
    {
      var insertIndex = dropInfo.InsertIndex;
      var destinationList = GetList(dropInfo.TargetCollection);
      var data = ExtractData(dropInfo.Data);

      var ic = dropInfo.VisualTarget as ItemsControl;
      if (ic != null && ic.Items.Count != destinationList.Count) { //then the Items are filtered
        if (insertIndex >= 0 && insertIndex < ic.Items.Count) {
          insertIndex = destinationList.IndexOf(ic.Items[insertIndex]);
        }
      }

      if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget) {
        var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

        foreach (var o in data) {
          var index = sourceList.IndexOf(o);

          if (index != -1) {
            sourceList.RemoveAt(index);

            if (sourceList == destinationList && index < insertIndex) {
              --insertIndex;
            }
          }
        }
      }
      else {
        var sourceList = GetList(dropInfo.DragInfo.SourceCollection);
        if (sourceList[0] is ICloneable) {
          var clones = new ArrayList();
          foreach (ICloneable o in data) {
            clones.Add(o.Clone());
          }
          data = clones;
        }
      }

      foreach (var o in data) {
        destinationList.Insert(insertIndex++, o);
      }
    }

    public static bool CanAcceptData(IDropInfo dropInfo)
    {
      if (dropInfo == null || dropInfo.DragInfo == null) {
        return false;
      }

      if (!String.IsNullOrEmpty((string)dropInfo.DragInfo.VisualSource.GetValue(DragDrop.DragDropContextProperty)))
      {
          //Source element has a drag context constraint, we need to check the target property matches.
          var sourceContext = (string)dropInfo.DragInfo.VisualSource.GetValue(DragDrop.DragDropContextProperty);
          var targetContext = (string)dropInfo.VisualTarget.GetValue(DragDrop.DragDropContextProperty);

          if (!string.Equals(sourceContext, targetContext))
              return false;
      }

      if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection) {
        return GetList(dropInfo.TargetCollection) != null;
      } else if (dropInfo.DragInfo.SourceCollection is ItemCollection) {
        return false;
      } else if (dropInfo.TargetCollection == null) {
        return false;
      } else {
        if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data)) {
          return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
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

    public static IList GetList(IEnumerable enumerable)
    {
      if (enumerable is ICollectionView) {
        return ((ICollectionView)enumerable).SourceCollection as IList;
      } else {
        return enumerable as IList;
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