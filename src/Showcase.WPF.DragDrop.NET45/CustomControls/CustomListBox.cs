using System.Windows;
using System.Windows.Controls;

namespace Showcase.WPF.DragDrop.CustomControls
{
  public class CustomListBox : ListBox
  {
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new CustomListBoxItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is CustomListBoxItem;
    }
  }

  public class CustomListBoxItem : ListBoxItem
  {
    public override string ToString()
    {
      return base.ToString();
    }
  }
}