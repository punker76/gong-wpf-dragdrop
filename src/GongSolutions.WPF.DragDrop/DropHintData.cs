namespace GongSolutions.Wpf.DragDrop;

/// <summary>
/// Data presented in drop hint adorner.
/// </summary>
public class DropHintData
{
    public DropHintData(DropHintState hintState, string hintText)
    {
        this.HintState = hintState;
        this.HintText = hintText;
    }

    /// <summary>
    /// The hint text to display to the user. See <see cref="IDropInfo.DropHintText"/>
    /// and <see cref="IDropHintInfo.DropHintText"/>.
    /// </summary>
    public string HintText { get; set; }
    /// <summary>
    /// The hint state to display different colors for hints. See <see cref="IDropInfo.DropTargetHintState"/>
    /// and <see cref="IDropHintInfo.DropTargetHintState"/>.
    /// </summary>
    public DropHintState HintState { get; set; }
}