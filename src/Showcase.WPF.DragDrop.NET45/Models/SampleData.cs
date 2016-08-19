using System.Collections.ObjectModel;

namespace Showcase.WPF.DragDrop.Models
{
  public class SampleData
  {
    public SampleData()
    {
      for (var n = 0; n < 50; ++n)
      {
        this.Collection1.Add($"Item {(n + 1)}");
      }

      for (var g = 0; g < 4; ++g)
      {
        for (var i = 0; i < ((g%2) == 0 ? 4 : 2); ++i)
        {
          this.GroupedCollection.Add(new GroupedItem(g, i));
        }
      }

      for (int r = 1; r <= 6; r++)
      {
        var root = new TreeNode($"Root {r}");
        for (var i = 0; i < ((r%2) == 0 ? 8 : 3); ++i)
        {
          root.Children.Add(new TreeNode($"Item {i + 10*r}"));
        }
        this.TreeCollection1.Add(root);
        if (r == 2)
        {
          root.IsExpanded = true;
        }
      }
    }

    public ObservableCollection<string> Collection1 { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> Collection2 { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> Collection3 { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<TreeNode> TreeCollection1 { get; set; } = new ObservableCollection<TreeNode>();
    public ObservableCollection<TreeNode> TreeCollection2 { get; set; } = new ObservableCollection<TreeNode>();

    public GroupedDropHandler GroupedDropHandler { get; set; } = new GroupedDropHandler();
    public ObservableCollection<GroupedItem> GroupedCollection { get; set; } = new ObservableCollection<GroupedItem>();
  }
}