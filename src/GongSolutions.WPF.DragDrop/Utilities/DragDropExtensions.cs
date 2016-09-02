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
  }
}