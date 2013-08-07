using System.Windows;
using System.Windows.Controls;

namespace DefaultsExample
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
}