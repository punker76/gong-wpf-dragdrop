using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Reflection;
using System.Collections;
using System.Windows.Controls.Primitives;

#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#endif

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  public static class ItemsControlExtensions
  {
    public static CollectionViewGroup FindGroup(this ItemsControl itemsControl, Point position)
    {
      if (itemsControl.Items.Groups == null || itemsControl.Items.Groups.Count == 0)
      {
        return null;
      }

      var element = itemsControl.InputHitTest(position) as DependencyObject;
      if (element != null)
      {
        var groupItem = element.GetVisualAncestor<GroupItem>();

        // drag after last item - get group of it
        if (groupItem == null && itemsControl.Items.Count > 0)
        {
          var lastItem = itemsControl.ItemContainerGenerator.ContainerFromItem(itemsControl.Items.GetItemAt(itemsControl.Items.Count - 1)) as FrameworkElement;
          if (lastItem != null)
          {
            var itemEndpoint = lastItem.PointToScreen(new Point(lastItem.ActualWidth, lastItem.ActualHeight));
            var positionToScreen = itemsControl.PointToScreen(position);
            switch (itemsControl.GetItemsPanelOrientation())
            {
              case Orientation.Horizontal:
                // assume LeftToRight
                groupItem = itemEndpoint.X <= positionToScreen.X ? lastItem.GetVisualAncestor<GroupItem>() : null;
                break;
              case Orientation.Vertical:
                groupItem = itemEndpoint.Y <= positionToScreen.Y ? lastItem.GetVisualAncestor<GroupItem>() : null;
                break;
            }
          }
        }
        if (groupItem != null)
        {
          return groupItem.Content as CollectionViewGroup;
        }
      }

      return null;
    }

    public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
    {
      if (itemsControl is MultiSelector) {
        // The CanSelectMultipleItems property is protected. Use reflection to
        // get its value anyway.
        return (bool)itemsControl.GetType()
                                 .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic)
                                 .GetValue(itemsControl, null);
      } else if (itemsControl is ListBox) {
        return ((ListBox)itemsControl).SelectionMode != SelectionMode.Single;
      } else {
        return false;
      }
    }

    public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
    {
      bool isItemContainer;
      var itemType = GetItemContainerType(itemsControl, out isItemContainer);

      if (itemType != null) {
        return isItemContainer
                 ? (UIElement)child.GetVisualAncestor(itemType, itemsControl)
                 : (UIElement)child.GetVisualAncestor(itemType, itemsControl.GetType());
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
    {
      var inputElement = itemsControl.InputHitTest(position);
      var uiElement = inputElement as UIElement;

      if (uiElement != null) {
        return GetItemContainer(itemsControl, uiElement);
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position,
                                               Orientation searchDirection)
    {
      bool isItemContainer;
      var itemContainerType = GetItemContainerType(itemsControl, out isItemContainer);

      Geometry hitTestGeometry;

      if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType)) {
        hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
      } else {
        var geometryGroup = new GeometryGroup();
        geometryGroup.Children.Add(new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y)));
        geometryGroup.Children.Add(new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height)));
        hitTestGeometry = geometryGroup;
      }

      var hits = new HashSet<DependencyObject>();

      VisualTreeHelper.HitTest(itemsControl,
        obj =>
        {
          if (obj is Viewport3D)
          {
            return HitTestFilterBehavior.Stop;
          }
          return HitTestFilterBehavior.Continue;
        },
        result =>
        {
          var itemContainer = isItemContainer
            ? result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl)
            : result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl.GetType());
          if (itemContainer != null && ((UIElement) itemContainer).IsVisible == true)
          {
            var tvItem = itemContainer as TreeViewItem;
            if (tvItem != null)
            {
              var tv = tvItem.GetVisualAncestor<TreeView>();
              if (tv == itemsControl)
              {
                hits.Add(itemContainer);
              }
            }
            else
            {
              if (itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) >= 0)
              {
                hits.Add(itemContainer);
              }
            }
          }
          return HitTestResultBehavior.Continue;
        },
        new GeometryHitTestParameters(hitTestGeometry));

      return GetClosest(itemsControl, hits, position, searchDirection);
    }

    public static Type GetItemContainerType(this ItemsControl itemsControl, out bool isItemContainer)
    {
      // determines if the itemsControl is not a ListView, ListBox or TreeView
      isItemContainer = false;

      if (typeof(TabControl).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(TabItem);
      }

      if (typeof(DataGrid).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(DataGridRow);
      }

      // There is no safe way to get the item container type for an ItemsControl. 
      // First hard-code the types for the common ItemsControls.
      //if (itemsControl.GetType().IsAssignableFrom(typeof(ListView)))
      if (typeof(ListView).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(ListViewItem);
      }
        //if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
      else if (typeof(ListBox).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(ListBoxItem);
      }
        //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
      else if (typeof(TreeView).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(TreeViewItem);
      }

      // Otherwise look for the control's ItemsPresenter, get it's child panel and the first 
      // child of that *should* be an item container.
      //
      // If the control currently has no items, we're out of luck.
      if (itemsControl.Items.Count > 0) {
        var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

        foreach (var itemsPresenter in itemsPresenters) {
          if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0) {
            var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
            var itemContainer = VisualTreeHelper.GetChildrenCount(panel) > 0
              ? VisualTreeHelper.GetChild(panel, 0)
              : null;

            // Ensure that this actually *is* an item container by checking it with
            // ItemContainerGenerator.
            if (itemContainer != null &&
                itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1) {
              isItemContainer = true;
              return itemContainer.GetType();
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the Orientation which will be used for the drag drop action.
    /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
    /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
    /// If so, the ItemsPanelOrientation value is taken.
    /// </summary>
    /// <param name="itemsControl">The ItemsControl for the look up.</param>
    /// <returns>Orientation for the given ItemsControl.</returns>
    public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
    {
      var itemsPanelOrientation = DragDrop.GetItemsPanelOrientation(itemsControl);
      if (itemsPanelOrientation.HasValue)
      {
        return itemsPanelOrientation.Value;
      }

      if (itemsControl is TabControl)
      {
        //HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)
        //var tabPanel = itemsControl.GetVisualDescendent<TabPanel>();
        var tabControl = (TabControl)itemsControl;
        return tabControl.TabStripPlacement == Dock.Left || tabControl.TabStripPlacement == Dock.Right ? Orientation.Vertical : Orientation.Horizontal;
      }
      
      var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>() ?? itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement;
      if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0) {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));
        if (orientationProperty != null) {
          return (Orientation)orientationProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return Orientation.Vertical;
    }

    /// <summary>
    /// Gets the FlowDirection which will be used for the drag drop action.
    /// </summary>
    /// <param name="itemsControl">The ItemsControl for the look up.</param>
    /// <returns>FlowDirection for the given ItemsControl.</returns>
    public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>() ?? itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement;
      if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0) {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));
        if (flowDirectionProperty != null) {
          return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return FlowDirection.LeftToRight;
    }

    public static void SetSelectedItem(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector) {
        ((MultiSelector)itemsControl).SelectedItem = null;
        ((MultiSelector)itemsControl).SelectedItem = item;
      } else if (itemsControl is ListBox) {
        var selectionMode = ((ListBox)itemsControl).SelectionMode;
        try {
          // change SelectionMode for UpdateAnchorAndActionItem
          ((ListBox)itemsControl).SelectionMode = SelectionMode.Single;
          ((ListBox)itemsControl).SelectedItem = null;
          ((ListBox)itemsControl).SelectedItem = item;
        }
        finally {
          ((ListBox)itemsControl).SelectionMode = selectionMode;
        }
      } else if (itemsControl is TreeView) {
        ((TreeView)itemsControl).SetValue(TreeView.SelectedItemProperty, null);
        ((TreeView)itemsControl).SetValue(TreeView.SelectedItemProperty, item);
      } else if (itemsControl is Selector) {
        ((Selector)itemsControl).SelectedItem = null;
        ((Selector)itemsControl).SelectedItem = item;
      }
    }

    public static object GetSelectedItem(this ItemsControl itemsControl)
    {
      if (itemsControl is MultiSelector) {
        return ((MultiSelector)itemsControl).SelectedItem;
      } else if (itemsControl is ListBox) {
        return ((ListBox)itemsControl).SelectedItem;
      } else if (itemsControl is TreeView) {
        return ((TreeView)itemsControl).GetValue(TreeView.SelectedItemProperty);
      } else if (itemsControl is Selector) {
        return ((Selector)itemsControl).SelectedItem;
      }
      return null;
    }

    public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
    {
      //if (itemsControl.GetType().IsAssignableFrom(typeof(MultiSelector)))
      if (typeof(MultiSelector).IsAssignableFrom(itemsControl.GetType())) {
        return ((MultiSelector)itemsControl).SelectedItems;
      } else if (itemsControl is ListBox) {
        var listBox = (ListBox)itemsControl;

        if (listBox.SelectionMode == SelectionMode.Single) {
          return Enumerable.Repeat(listBox.SelectedItem, 1);
        } else {
          return listBox.SelectedItems;
        }
      }
        //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
      else if (typeof(TreeView).IsAssignableFrom(itemsControl.GetType())) {
        return Enumerable.Repeat(((TreeView)itemsControl).SelectedItem, 1);
      }
        //else if (itemsControl.GetType().IsAssignableFrom(typeof(Selector)))
      else if (typeof(Selector).IsAssignableFrom(itemsControl.GetType())) {
        return Enumerable.Repeat(((Selector)itemsControl).SelectedItem, 1);
      } else {
        return Enumerable.Empty<object>();
      }
    }

    public static bool GetItemSelected(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector) {
        return ((MultiSelector)itemsControl).SelectedItems.Contains(item);
      } else if (itemsControl is ListBox) {
        return ((ListBox)itemsControl).SelectedItems.Contains(item);
      } else if (itemsControl is TreeView) {
        return ((TreeView)itemsControl).SelectedItem == item;
      } else if (itemsControl is Selector) {
        return ((Selector)itemsControl).SelectedItem == item;
      } else {
        return false;
      }
    }

    public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
    {
      if (itemsControl is MultiSelector) {
        var multiSelector = (MultiSelector)itemsControl;

        if (value) {
          if (multiSelector.CanSelectMultipleItems()) {
            multiSelector.SelectedItems.Add(item);
          } else {
            multiSelector.SelectedItem = item;
          }
        } else {
          multiSelector.SelectedItems.Remove(item);
        }
      } else if (itemsControl is ListBox) {
        var listBox = (ListBox)itemsControl;

        if (value) {
          if (listBox.SelectionMode != SelectionMode.Single) {
            listBox.SelectedItems.Add(item);
          } else {
            listBox.SelectedItem = item;
          }
        } else {
          listBox.SelectedItems.Remove(item);
        }
      } else {
        if (value) {
          itemsControl.SetSelectedItem(item);
        } else {
          itemsControl.SetSelectedItem(null);
        }
      }
    }

    private static UIElement GetClosest(ItemsControl itemsControl, IEnumerable<DependencyObject> items,
                                        Point position, Orientation searchDirection)
    {
      //Console.WriteLine("GetClosest - {0}", itemsControl.ToString());

      UIElement closest = null;
      var closestDistance = double.MaxValue;

      foreach (var i in items) {
        var uiElement = i as UIElement;

        if (uiElement != null) {
          var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
          var distance = double.MaxValue;

          if (itemsControl is TreeView) {
            var xDiff = position.X - p.X;
            var yDiff = position.Y - p.Y;
            var hyp = Math.Sqrt(Math.Pow(xDiff, 2d) + Math.Pow(yDiff, 2d));
            distance = Math.Abs(hyp);
          } else {
            var itemParent = ItemsControl.ItemsControlFromItemContainer(uiElement);
            if (itemParent != null && itemParent != itemsControl)
            {
              searchDirection = itemParent.GetItemsPanelOrientation();
            }
            switch (searchDirection) {
              case Orientation.Horizontal:
                distance = position.X <= p.X ? p.X - position.X : position.X - uiElement.RenderSize.Width - p.X;
                break;
              case Orientation.Vertical:
                distance = position.Y <= p.Y ? p.Y - position.Y : position.Y - uiElement.RenderSize.Height - p.Y;
                break;
            }
          }

          if (distance < closestDistance) {
            closest = uiElement;
            closestDistance = distance;
          }
        }
      }

      return closest;
    }
  }
}