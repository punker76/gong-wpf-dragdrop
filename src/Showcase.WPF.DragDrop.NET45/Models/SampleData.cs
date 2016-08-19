using System.Collections.ObjectModel;

namespace Showcase.WPF.DragDrop.Models
{
  public class SampleData
  {
    public SampleData()
    {
      this.Collection1 = new ObservableCollection<string>();
      this.Collection2 = new ObservableCollection<string>();
      this.Collection3 = new ObservableCollection<string>();

      for (var n = 0; n < 50; ++n)
      {
        this.Collection1.Add($"Item {(n + 1)}");
      }

      this.GroupedDropHandler = new GroupedDropHandler();
      this.GroupedCollection = new ObservableCollection<GroupedItem>();
      for (var g = 0; g < 4; ++g)
      {
        for (var i = 0; i < ((g % 2) == 0 ? 4 : 2); ++i)
        {
          this.GroupedCollection.Add(new GroupedItem(g, i));
        }
      }
    }

    public ObservableCollection<string> Collection1 { get; set; }
    public ObservableCollection<string> Collection2 { get; set; }
    public ObservableCollection<string> Collection3 { get; set; }

    public GroupedDropHandler GroupedDropHandler { get; set; }
    public ObservableCollection<GroupedItem> GroupedCollection { get; set; }
  }
}