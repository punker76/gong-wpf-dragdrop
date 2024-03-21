namespace Showcase.WPF.DragDrop.Models;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;

public class SpecializedDropErrorAdorner : DropTargetHintAdorner
{
    public SpecializedDropErrorAdorner(UIElement adornedElement, IDropHintInfo dropHintInfo, DataTemplate dataTemplate)
        : base(adornedElement, dropHintInfo, null)
    {
        Background = new SolidColorBrush(Colors.DarkRed) { Opacity = 0.3 };
        Pen = new Pen(Brushes.DarkRed, 0.5);
        Background.Freeze();
    }
}

public class SpecializedDropHintTarget : IDropTarget
{
    public void DropHintOver(IDropHintInfo dropHintInfo)
    {
        if (!this.CanAccept(dropHintInfo.DragInfo) || dropHintInfo.DropInfo?.TargetItem is not DestinationModel)
        {
            dropHintInfo.DestinationText ="Can't drop here";
            // Red adorner if we can't drop
            dropHintInfo.DropTargetHintAdorner = typeof(SpecializedDropErrorAdorner);
        }
    }

    public void DropHint(IDropHintInfo dropHintInfo)
    {
        if(CanAccept(dropHintInfo.DragInfo))
        {
            dropHintInfo.DestinationText = "Send to destination";

            dropHintInfo.DropTargetHintAdorner = typeof(DropTargetHintAdorner);
        }
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (this.CanAccept(dropInfo.DragInfo))
        {
            var copyData = DefaultDropHandler.ShouldCopyData(dropInfo);
            if (dropInfo.VisualTargetItem is ListBoxItem)
            {
                dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.EffectText = "Send";
                dropInfo.DestinationText = $"Send to {(dropInfo.TargetItem as DestinationModel)?.Name}".Trim();
            }
        }
    }

    private bool CanAccept(IDragInfo dragInfo)
    {
        if (dragInfo == null)
        {
            return false;
        }

        var items = DefaultDropHandler.ExtractData(dragInfo.Data)
                                      .OfType<ItemModel>()
                                      .ToList();
        return items.Count > 0;
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.TargetItem is DestinationModel destination)
        {
            destination.Count++;
        }
    }
}