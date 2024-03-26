namespace Showcase.WPF.DragDrop.Models;

using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

public class CustomDropHintHandler : DefaultDropHandler
{
    /// <summary>
    /// When false, will set red border and hint text to "Drop not allowed for this element"
    /// </summary>
    public bool IsDropAllowed { get; set; } = true;
    public bool BlockOdd { get; set; }

    /// <summary>
    /// Do not display active hint on mouse over.
    /// </summary>
    public bool IsActiveHintDisabled { get; set; }

    public override void DropHint(IDropHintInfo dropHintInfo)
    {
        if (!this.CanAccept(dropHintInfo.DragInfo))
        {
            return;
        }

        if (!this.IsDropAllowed)
        {
            dropHintInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropHintInfo.DropTargetHintState = DropHintState.Error;
            dropHintInfo.DropHintText = "Drop not allowed for this element";
        }
        else
        {
            dropHintInfo.DropHintText = "Drop data here";
            dropHintInfo.DropTargetHintAdorner = typeof(DropTargetHintAdorner);
        }
    }

    public override void DragOver(IDropInfo dropInfo)
    {
        if (!this.CanAccept(dropInfo.DragInfo))
        {
            return;
        }

        if (!IsDropAllowed)
        {
            dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropInfo.DropTargetHintState = DropHintState.Error;
            dropInfo.DropHintText = "Drop not allowed for this element";
            return;
        }

        if (BlockOdd && dropInfo.DragInfo.SourceItem is ItemModel item && item.Index % 2 != 0)
        {
            dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropInfo.DropTargetHintState = DropHintState.Error;
            dropInfo.DropHintText = "Only items with even index is allowed";
            dropInfo.Effects = DragDropEffects.None;
            return;
        }

        var copyData = ShouldCopyData(dropInfo);

        dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.EffectText = "Send";
        if(IsActiveHintDisabled)
        {
            // No drag over hint
            return;
        }

        dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
        dropInfo.DropHintText = $"Dropping {(dropInfo.DragInfo.SourceItem as ItemModel)?.Caption} on {(dropInfo.TargetItem as ItemModel)?.Caption}";
        dropInfo.DropTargetHintState = DropHintState.Active;
    }


    private bool CanAccept(IDragInfo dragInfo)
    {
        if (dragInfo == null)
        {
            return false;
        }

        var items = ExtractData(dragInfo.Data)
                                      .OfType<ItemModel>()
                                      .ToList();
        return items.Count > 0;
    }
}