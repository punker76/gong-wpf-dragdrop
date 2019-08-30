using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
    public class ListBoxCustomDropHandler : DefaultDropHandler
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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