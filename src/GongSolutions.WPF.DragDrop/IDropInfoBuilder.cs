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
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// <param name="sender">The sender of the drop event.</param>
        /// <param name="e">The drag event arguments.</param>
        /// <param name="dragInfo">Information about the drag source, if the drag came from within the framework.</param>
        /// <param name="eventType">The type of the underlying event (tunneled or bubbled).</param>
        [CanBeNull]
        IDropInfo CreateDropInfo(object sender, DragEventArgs e, [CanBeNull] DragInfo dragInfo, EventType eventType);
    }
}