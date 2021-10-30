using System;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Interface implemented by Drag Info Builders.
    /// It enables custom construction of DragInfo objects to support 3rd party controls like DevExpress, Telerik, etc.
    /// </summary>
    public interface IDragInfoBuilder
    {
        /// <summary>
        /// Initializes a new instance of the DragInfo class.
        /// </summary>
        /// <param name="sender">The sender of the input event that initiated the drag operation.</param>
        /// <param name="originalSource">The original source of the input event.</param>
        /// <param name="mouseButton">The mouse button which was used for the drag operation.</param>
        /// <param name="getPosition">A function of the input event which is used to get drag position points.</param>
        [CanBeNull]
        DragInfo CreateDragInfo(object sender, object originalSource, MouseButton mouseButton, Func<IInputElement, Point> getPosition);
    }
}