using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
  public class ListBoxCustomDropHandler : DefaultDropHandler
  {
    public override void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
      {
        dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
      }
      else
      {
        base.DragOver(dropInfo);
      }
    }

    public override void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
      {
        dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
      }
      else
      {
        base.Drop(dropInfo);
      }
    }
  }
}