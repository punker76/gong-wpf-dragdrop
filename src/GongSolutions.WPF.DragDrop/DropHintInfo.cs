namespace GongSolutions.Wpf.DragDrop;

using System;

/// <summary>
/// Implementation of the <see cref="IDropHintInfo"/> interface to hold DropHint information.
/// </summary>
public class DropHintInfo : IDropHintInfo
{
    /// <inheritdoc />
    public IDragInfo DragInfo { get; }
    /// <inheritdoc />
    public IDropInfo DropInfo { get; }

    /// <inheritdoc />
    public Type DropTargetHintAdorner { get; set; }

    /// <inheritdoc />
    public string DestinationText { get; set; }

    public DropHintInfo(IDragInfo dragInfo, IDropInfo dropInfo = null)
    {
        this.DragInfo = dragInfo;
        this.DropInfo = dropInfo;
    }
}