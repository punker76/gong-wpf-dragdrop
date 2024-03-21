namespace GongSolutions.Wpf.DragDrop;

using System;

/// <summary>
/// This interface is used with the <see cref="IDropTarget.DropHint"/> and <see cref="IDropTarget.DropHintOver"/> methods for
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
    /// Get the drop info holding information about the current drag state.
    /// </summary>
    IDropInfo DropInfo { get; }

    /// <summary>
    /// Gets or sets the class of drop target hint to display.
    /// </summary>
    /// <remarks>
    /// The standard drop target Adorner classes are held in the <see cref="DropTargetAdorners"/>
    /// class.
    /// </remarks>
    Type DropTargetHintAdorner { get; set; }

    string DestinationText { get; set; }
}