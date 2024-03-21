namespace Showcase.WPF.DragDrop.Models;

using System.Collections.Generic;
using System.Linq;
using GongSolutions.Wpf.DragDrop;

/// <summary>
/// Drop target displaying hint about the item dropped.
/// </summary>
public class HintDropTarget : DefaultDropHandler
{
    public override void DropHintOver(IDropHintInfo dropHintInfo)
    {
        DropHint(dropHintInfo);
    }

    public override void DropHint(IDropHintInfo dropHintInfo)
    {
        var items = GetData(dropHintInfo);
        if(items.Count > 1)
        {
            dropHintInfo.DestinationText = $"Drop {items.Count} items";
        }
        else if(items.Count == 1)
        {
            var item = items[0];
            dropHintInfo.DestinationText = item.Index % 2 == 0 ? "Drop even" : "Drop odd";
        }

        dropHintInfo.DropTargetHintAdorner = typeof(DropTargetHintAdorner);
    }

    private static List<ItemModel> GetData(IDropHintInfo dropHintInfo)
    {
        var items = ExtractData(dropHintInfo.DragInfo?.Data)
                    .OfType<ItemModel>()
                    .ToList();
        return items;
    }
}