namespace Showcase.WPF.DragDrop.Models;

using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

/// <summary>
/// Drop info builder for ComboBox to redirect drop information to the ComboBox itself instead
/// of the internal TextBox.
/// </summary>
/// <param name="comboBox"></param>
public class ComboBoxDropInfoBuilder(ComboBox comboBox) : IDropInfoBuilder
{
    public IDropInfo CreateDropInfo(object sender, DragEventArgs e, IDragInfo dragInfo, EventType eventType)
    {
        if (!comboBox.IsEditable)
        {
            return null;
        }

        // If the target is the TextBox inside ComboBox, we want to create
        // drop info as if the drop happened on the ComboBox itself
        if (e.Source is TextBox || (e.Source is UIElement element && element.GetVisualAncestor<TextBox>() != null))
        {
            e.Source = comboBox;
            e.Handled = true;
            return new DropInfo(comboBox, e, dragInfo, eventType);
        }

        return null;
    }
}
