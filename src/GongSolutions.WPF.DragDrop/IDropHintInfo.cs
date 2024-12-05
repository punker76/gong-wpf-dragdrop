using System;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// This interface is used with the <see cref="IDropTarget.DropHint"/> for
    /// hint to the user about potential drop targets.
    /// </summary>
    public interface IDropHintInfo
    {
        /// <summary>
        /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag,
        /// if the drag came from within the framework.
        /// </summary>
        IDragInfo DragInfo { get; }

        /// <summary>
        /// Gets or sets the class of drop target hint to display.
        /// </summary>
        /// <remarks>
        /// The standard drop target Adorner classes are held in the <see cref="DropTargetAdorners"/>
        /// class.
        /// </remarks>
        Type DropTargetHintAdorner { get; set; }

        /// <summary>
        /// Get or set the text that is displayed when initial drop hint is displayed.
        /// </summary>
        /// <remarks>
        /// This corresponds to <see cref="IDropInfo.DropHintText"/> in <see cref="IDropTarget.DragEnter"/>
        /// and <see cref="IDropTarget.DragOver"/>.
        /// </remarks>
        string DropHintText { get; set; }

        /// <summary>
        /// The hint state to display different colors for hints.
        /// </summary>
        DropHintState DropTargetHintState { get; set; }
    }
}