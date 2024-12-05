namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Represents the mode of the drop hint to display different adorner based on the state of the hint.
    /// </summary>
    public enum DropHintState
    {
        /// <summary>
        /// Default hint state, indicating that a drop target is available for drop.
        /// </summary>
        None,
        /// <summary>
        /// Highlights the target, such as on drag over.
        /// </summary>
        Active,
        /// <summary>
        /// Warning state, indicating that the drop target is not available for drop.
        /// </summary>
        Error
    }
}