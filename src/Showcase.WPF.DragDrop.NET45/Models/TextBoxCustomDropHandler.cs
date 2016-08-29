using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
  public class TextBoxCustomDropHandler : IDropTarget
  {
    public void DragOver(IDropInfo dropInfo)
    {
      dropInfo.Effects = DragDropEffects.Move;
    }

    public void Drop(IDropInfo dropInfo)
    {
      var dataAsList = DefaultDropHandler.ExtractData(dropInfo.Data);
      ((TextBox)dropInfo.VisualTarget).Text = string.Join(", ", dataAsList.OfType<object>().ToArray());
    }
  }
}