using System.Windows;
using System.Windows.Controls;

namespace Showcase.WPF.DragDrop.Models
{
  public class DragAdornerTemplateSelector : DataTemplateSelector
  {
    public DataTemplate TemplateEven { get; set; }
    public DataTemplate TemplateOdd { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var itemModel = item as ItemModel;
      return itemModel != null && (itemModel.Index & 0x01) == 0 ? TemplateEven : TemplateOdd;
    }
  }
}