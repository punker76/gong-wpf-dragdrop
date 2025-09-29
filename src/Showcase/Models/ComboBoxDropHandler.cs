namespace Showcase.WPF.DragDrop.Models;

using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;

public class ComboBoxDropHandler : IDropTarget
{
    public void DragEnter(IDropInfo dropInfo)
    {
        // nothing here
    }

    public void DragLeave(IDropInfo dropInfo)
    {
        // nothing here
    }

    private bool IsAcceptedItem(IDragInfo data)
    {
        return data?.Data is ListBoxItem;
    }

    public void DropHint(IDropHintInfo dropHintInfo)
    {
        if (!IsAcceptedItem(dropHintInfo.DragInfo))
        {
            if (dropHintInfo.DragInfo == null)
            {
                return;
            }

            dropHintInfo.DropHintText = "Cannot drop here";
            dropHintInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropHintInfo.DropTargetHintState = DropHintState.Error;
            return;
        }

        dropHintInfo.DropHintText = "Drop here";
        dropHintInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (!IsAcceptedItem(dropInfo.DragInfo))
        {
            if (dropInfo.DragInfo == null)
            {
                return;
            }

            dropInfo.DropHintText = "Cannot drop here";
            dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropInfo.DropTargetHintState = DropHintState.Error;
            return;
        }

        dropInfo.DropHintText = "Let go!";
        dropInfo.DropTargetHintState = DropHintState.Active;
        dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
        dropInfo.Effects = DragDropEffects.Copy;
        dropInfo.EffectText = "drop";
        dropInfo.DestinationText = "Drop";
    }

    public void Drop(IDropInfo dropInfo)
    {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Hint;
        dropInfo.Effects = DragDropEffects.Copy;
    }
}
