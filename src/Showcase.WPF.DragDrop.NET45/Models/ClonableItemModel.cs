using System;

namespace Showcase.WPF.DragDrop.Models
{
  public class ClonableItemModel : ItemModel, ICloneable
  {
    public ClonableItemModel()
    {
    }

    public ClonableItemModel(int itemIndex) : base(itemIndex)
    {
    }

    public object Clone()
    {
      var clonableItemModel = new ClonableItemModel();
      clonableItemModel.BindableDoubleValue = this.BindableDoubleValue;
      clonableItemModel.SubItemCollection.Clear();
      foreach (var subItem in this.SubItemCollection)
      {
        clonableItemModel.SubItemCollection.Add(subItem);
      }
      clonableItemModel.SelectedSubItem = this.SelectedSubItem;
      clonableItemModel.Index = this.Index;
      clonableItemModel.Caption = $"Cloned Item {this.Index}";
      return clonableItemModel;
    }
  }
}