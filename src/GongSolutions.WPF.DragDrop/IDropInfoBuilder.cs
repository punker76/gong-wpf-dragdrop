using System.Windows;
using JetBrains.Annotations;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Interface implemented by Drop Info Builders.
    /// It enables custom construction of IDropInfo objects to support 3rd party controls like DevExpress, Telerik, etc.
    /// </summary>
    public interface IDropInfoBuilder
    {
        /// <summary>
        /// Creates a drop info object from <see cref="DragEventArgs"/> and <see cref="DragInfo"/>.
        /// </summary>
        ///
        /// <param name="sender">
        /// The sender of the mouse event that initiated the drag.
        /// </param>
        /// 
        /// <param name="e">
        /// The drag event args that initiated the drag.
        /// </param>
        ///
        /// <param name="dragInfo">
        /// Object which contains the drag information.
        /// </param>
        ///
        /// <param name="eventType">
        /// The mode of the underlying routed event.
        /// </param>
        [CanBeNull] IDropInfo CreateDropInfo(object sender, [CanBeNull] DragEventArgs e, DragInfo dragInfo, EventType eventType);
    }
}