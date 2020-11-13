using System.Collections;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Interface for objects that sort an IEnumerable of drag drop items that are 
    /// going to be dropped on some target
    /// </summary>
    public interface IDropTargetItemsSorter
    {
        /// <summary>
        /// Sort the IEnumerable of items that are going to be dropped on a target
        /// </summary>
        /// <param name="items">Enumerable of dragged items to sort</param>
        /// <returns>The sorted list of dragged items</returns>
        IEnumerable SortDropTargetItems(IEnumerable items);
    }
}
