using System.Windows.Input;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Interface implemented by Drag Info Builders.
    /// It enables custom construction of DragInfo objects from 3rd party controls like DevExpress, Telerik, etc.
    /// </summary>
    public interface IDragInfoBuilder
    {
        /// <summary>
        /// Creates a drag info object from <see cref="MouseButtonEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the mouse event that initiated the drag.
        /// </param>
        /// 
        /// <param name="e">
        /// The mouse event that initiated the drag.
        /// </param>
        DragInfo CreateDragInfo(object sender, MouseButtonEventArgs e);

        /// <summary>
        /// Creates a drag info object from <see cref="TouchEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the touch event that initiated the drag.
        /// </param>
        /// 
        /// <param name="e">
        /// The touch event that initiated the drag.
        /// </param>
        DragInfo CreateDragInfo(object sender, TouchEventArgs e);
    }
}