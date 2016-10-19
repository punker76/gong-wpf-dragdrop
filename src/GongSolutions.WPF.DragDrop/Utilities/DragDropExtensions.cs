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
        /// Determines whether the given element can be dragged with the mouse right button (<see cref="DragDrop.CanDragWithMouseRightButton"/> --sulinke1133@gmail.com 
        /// </summary>
        /// <param name="element">The given element</param>
        /// <returns>Element can be dragged by right mouse button or not</returns>
        public static bool CanDragWithMouseRightButton(this UIElement element)
        {
            return element != null && DragDrop.GetCanDragWithMouseRightButton(element);
        }

  }
}