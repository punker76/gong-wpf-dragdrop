namespace Showcase.WPF.DragDrop.Models;

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

public class ComboBoxDropBehavior : Behavior<ComboBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= AssociatedObject_Loaded;
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        if (AssociatedObject.Template.FindName("PART_EditableTextBox", AssociatedObject) is not TextBox textBox)
        {
            return;
        }

        // Create and set the custom drop info builder
        var dropInfoBuilder = new ComboBoxDropInfoBuilder(AssociatedObject);
        DragDrop.SetDropInfoBuilder(textBox, dropInfoBuilder);

        // Attach Gong DragDrop manually
        DragDrop.SetIsDropTarget(textBox, DragDrop.GetIsDropTarget(AssociatedObject));
    }
}