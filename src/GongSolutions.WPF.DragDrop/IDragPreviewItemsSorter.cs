using System.Collections;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Interface for objects that sort an IEnumerable of drag preview items
    /// </summary>
    public interface IDragPreviewItemsSorter
    {
        /// <summary>
        /// Sort the IEnumerable of items that are being shown in a drag preview
        /// </summary>
        /// <param name="items">Enumerable of dragged items to sort</param>
        /// <returns>The sorted list of dragged items</returns>
        IEnumerable SortDragPreviewItems(IEnumerable items);
    }
}
