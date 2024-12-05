namespace Showcase.WPF.DragDrop.Models;

using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.IconPacks;

public class FilesDropHandler : DefaultDropHandler
{
    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is DropInfo { TargetItem: not TreeNode { Icon: PackIconMaterialKind.Folder } } typedDropInfo)
            typedDropInfo.AcceptChildItem = false;

        base.DragOver(dropInfo);
    }
}