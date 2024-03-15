namespace GongSolutions.Wpf.DragDrop;

/// <summary>
/// Interface implemented by Drop Handlers which can provide a hint to the user where the item can be dropped.
/// </summary>
public interface IDropTargetHint : IDropTarget
{
    /// <summary>
    /// Notifies the drop handler when a drag is initiated to display hint about potential drop targets.
    /// </summary>
    /// <param name="dropInfo">Object which contains several drop information.</param>
#if NETCOREAPP3_1_OR_GREATER
    void DropHint(IDropInfo dropInfo)
    {
        // nothing here
    }
#else
        void DropHint(IDropInfo dropInfo);
#endif
}