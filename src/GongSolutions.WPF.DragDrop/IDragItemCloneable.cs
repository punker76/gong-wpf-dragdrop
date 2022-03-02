namespace GongSolutions.Wpf.DragDrop
{
    public interface IDragItemCloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        object CloneDragItem(IDropInfo dropInfo);
    }
}