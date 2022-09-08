namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Supports methods for models which can be dropped.
    /// </summary>
    public interface IDragItemSource
    {
        /// <summary>
        /// Indicates that the item is dropped on the destination list.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
        void ItemDropped(IDropInfo dropInfo);
    }
}