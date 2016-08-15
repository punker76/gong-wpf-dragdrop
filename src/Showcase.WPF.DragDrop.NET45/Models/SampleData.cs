using System.Collections.ObjectModel;

namespace Showcase.WPF.DragDrop.Models
{
  public class SampleData
  {
    public SampleData()
    {
      this.Collection1 = new ObservableCollection<string>();
      this.Collection2 = new ObservableCollection<string>();
      for (var n = 0; n < 100; ++n)
      {
        this.Collection1.Add("Item " + (n + 1));
      }
    }

    public ObservableCollection<string> Collection1 { get; set; }
    public ObservableCollection<string> Collection2 { get; set; }
  }
}