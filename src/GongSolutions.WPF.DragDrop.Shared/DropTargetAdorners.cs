using System;

namespace GongSolutions.Wpf.DragDrop
{
    public class DropTargetAdorners
    {
        /// <summary>
        /// Gets the type of the default highlight target adorner.
        /// </summary>
        public static Type Highlight { get; } = typeof(DropTargetHighlightAdorner);

        /// <summary>
        /// Gets the type of the default insert target adorner.
        /// </summary>
        public static Type Insert { get; } = typeof(DropTargetInsertionAdorner);
    }
}