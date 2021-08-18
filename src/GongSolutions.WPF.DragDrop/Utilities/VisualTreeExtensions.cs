using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using JetBrains.Annotations;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Gets the next ancestor element which is a drop target.
        /// </summary>
        /// <param name="element">The start element.</param>
        /// <returns>The first element which is a drop target.</returns>
        public static UIElement TryGetNextAncestorDropTargetElement(this UIElement element)
        {
            if (element == null)
            {
                return null;
            }

            var ancestor = element.GetVisualAncestor<UIElement>();
            while (ancestor != null)
            {
                if (ancestor.IsDropTarget())
                {
                    return ancestor;
                }

                ancestor = ancestor.GetVisualAncestor<UIElement>();
            }

            return null;
        }

        internal static DependencyObject FindVisualTreeRoot(this DependencyObject d)
        {
            var current = d;
            var result = d;

            while (current != null)
            {
                result = current;
                if (current is Visual || current is Visual3D)
                {
                    break;
                }
                else
                {
                    // If we're in Logical Land then we must walk 
                    // up the logical tree until we find a 
                    // Visual/Visual3D to get us back to Visual Land.
                    current = LogicalTreeHelper.GetParent(current);
                }
            }

            return result;
        }

        public static T GetVisualAncestor<T>(this DependencyObject d)
            where T : class
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

            while (item != null)
            {
                var itemAsT = item as T;
                if (itemAsT != null)
                {
                    return itemAsT;
                }

                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        /// <summary>
        /// find the visual ancestor item by type
        /// </summary>
        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type itemSearchType, [NotNull] ItemsControl itemsControl, [NotNull] Type itemContainerSearchType)
        {
            if (itemsControl == null) throw new ArgumentNullException(nameof(itemsControl));
            if (itemContainerSearchType == null) throw new ArgumentNullException(nameof(itemContainerSearchType));

            var visualTreeRoot = d.FindVisualTreeRoot();
            var currentVisual = VisualTreeHelper.GetParent(visualTreeRoot);

            while (currentVisual != null && itemSearchType != null)
            {
                var currentVisualType = currentVisual.GetType();
                if (currentVisualType == itemSearchType || currentVisualType.IsSubclassOf(itemSearchType))
                {
                    if (currentVisual is TreeViewItem || itemsControl.ItemContainerGenerator.IndexFromContainer(currentVisual) != -1)
                    {
                        return currentVisual;
                    }
                }

                if (itemContainerSearchType.IsAssignableFrom(currentVisualType))
                {
                    // ok, we found an ItemsControl (maybe an empty)
                    return null;
                }

                currentVisual = VisualTreeHelper.GetParent(currentVisual);
            }

            return null;
        }

        /// <summary>
        /// find the visual ancestor by type and go through the visual tree until the given itemsControl will be found
        /// </summary>
        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type itemSearchType, [NotNull] ItemsControl itemsControl)
        {
            if (itemsControl == null) throw new ArgumentNullException(nameof(itemsControl));

            var visualTreeRoot = d.FindVisualTreeRoot();
            var currentVisual = VisualTreeHelper.GetParent(visualTreeRoot);
            DependencyObject lastFoundItemByType = null;

            while (currentVisual != null && itemSearchType != null)
            {
                if (currentVisual == itemsControl)
                {
                    return lastFoundItemByType;
                }

                var currentVisualType = currentVisual.GetType();
                if ((currentVisualType == itemSearchType || currentVisualType.IsSubclassOf(itemSearchType))
                    && (itemsControl.ItemContainerGenerator.IndexFromContainer(currentVisual) != -1))
                {
                    lastFoundItemByType = currentVisual;
                }

                currentVisual = VisualTreeHelper.GetParent(currentVisual);
            }

            return lastFoundItemByType;
        }

        public static T GetVisualDescendent<T>(this DependencyObject d)
            where T : DependencyObject
        {
            return d.GetVisualDescendents<T>(null).FirstOrDefault();
        }

        public static T GetVisualDescendent<T>(this DependencyObject d, string childName)
            where T : DependencyObject
        {
            return d.GetVisualDescendents<T>(childName).FirstOrDefault();
        }

        public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d)
            where T : DependencyObject
        {
            return d.GetVisualDescendents<T>(null);
        }

        public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d, string childName)
            where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var n = 0; n < childCount; n++)
            {
                var child = VisualTreeHelper.GetChild(d, n);

                if (child is T descendent)
                {
                    if (string.IsNullOrEmpty(childName)
                        || descendent is IFrameworkInputElement frameworkInputElement && frameworkInputElement.Name == childName)
                    {
                        yield return descendent;
                    }
                }

                foreach (var match in GetVisualDescendents<T>(child, childName))
                {
                    yield return match;
                }
            }

            yield break;
        }

        /// <summary>
        /// GetVisibleDescendantBounds returns the union of all of the content bounding boxes of the specified Visual's sub-graph.
        /// It's a work around of VisualTreeHelper.GetDescendantBounds() including collapsed Visuals in its bounds calculations.
        /// </summary>
        public static Rect GetVisibleDescendantBounds(Visual visual)
        {
            return VisualTreeDescendantBoundsHelper.GetVisibleDescendantBounds(visual);
        }
    }
}