using System.Windows;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class DragDropExtensions
    {
        /// <summary>
        /// Determines whether the given element is ignored on drag start (<see cref="DragDrop.DragSourceIgnore"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDragSourceIgnored(this UIElement element)
        {
            return element != null && DragDrop.GetDragSourceIgnore(element);
        }

        /// <summary>
        /// Determines whether the given element is ignored on drop action (<see cref="DragDrop.IsDragSource"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDragSource(this UIElement element)
        {
            return element != null && DragDrop.GetIsDragSource(element);
        }

        /// <summary>
        /// Determines whether the given element is ignored on drop action (<see cref="DragDrop.IsDropTarget"/>).
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <returns>Element is ignored or not.</returns>
        public static bool IsDropTarget(this UIElement element)
        {
            return element != null && DragDrop.GetIsDropTarget(element);
        }
        
        /// <summary>
        /// Gets if drop position is directly over element
        /// </summary>
        /// <param name="dropPosition">Drop position</param>
        /// <param name="element">element to check whether or not the drop position is directly over or not</param>
        /// <param name="relativeToElement">element to which the drop position is related</param>
        /// <returns>drop position is directly over element or not</returns>
        public static bool DirectlyOverElement(this Point dropPosition, UIElement element, UIElement relativeToElement)
        {
            if (element == null)
                return false;

            var relativeItemPosition = element.TranslatePoint(new Point(0, 0), relativeToElement);
            var relativeDropPosition = new Point(dropPosition.X - relativeItemPosition.X, dropPosition.Y - relativeItemPosition.Y);
            return VisualTreeHelper.GetDescendantBounds(element).Contains(relativeDropPosition); 
        }
    }
}
