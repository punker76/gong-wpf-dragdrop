using System;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
    public class NestedDropHandler : IDropTarget
    {
        /// <inheritdoc />
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.TargetItem?.ToString().StartsWith("Root", StringComparison.OrdinalIgnoreCase) == true)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            }
        }

        /// <inheritdoc />
        public void Drop(IDropInfo dropInfo)
        {
            // nothing
        }
    }
}