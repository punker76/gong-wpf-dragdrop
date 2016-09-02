using System.Collections.ObjectModel;

namespace Showcase.WPF.DragDrop.Models
{
  public class SampleData
  {
    public SampleData()
    {
      for (var n = 0; n < 50; ++n)
      {
        this.Collection1.Add(new ItemModel(n + 1));
        this.FilterCollection1.Add(new ItemModel(n + 1));
        this.ClonableCollection1.Add(new ClonableItemModel(n + 1));
        this.DataGridCollection1.Add(new DataGridRowModel());
      }
      for (var n = 0; n < 10; ++n)
      {
        this.Collection4.Add(new ItemModel() {Caption = $"Model {n + 1}"});
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

      for (int i = 0; i < 5; i++)
      {
        this.TabItemCollection1.Add(new TabItemModel(i + 1));
      }
      this.TabItemCollection2.Add(new TabItemModel(1));
    }

    public ObservableCollection<ItemModel> Collection1 { get; set; } = new ObservableCollection<ItemModel>();
    public ObservableCollection<ItemModel> Collection2 { get; set; } = new ObservableCollection<ItemModel>();
    public ObservableCollection<ItemModel> Collection3 { get; set; } = new ObservableCollection<ItemModel>();
    public ObservableCollection<ItemModel> Collection4 { get; set; } = new ObservableCollection<ItemModel>();

    public ObservableCollection<ClonableItemModel> ClonableCollection1 { get; set; } = new ObservableCollection<ClonableItemModel>();
    public ObservableCollection<ClonableItemModel> ClonableCollection2 { get; set; } = new ObservableCollection<ClonableItemModel>();

    public ObservableCollection<ItemModel> FilterCollection1 { get; set; } = new ObservableCollection<ItemModel>();
    public ObservableCollection<ItemModel> FilterCollection2 { get; set; } = new ObservableCollection<ItemModel>();

    public ObservableCollection<TreeNode> TreeCollection1 { get; set; } = new ObservableCollection<TreeNode>();
    public ObservableCollection<TreeNode> TreeCollection2 { get; set; } = new ObservableCollection<TreeNode>();

    public GroupedDropHandler GroupedDropHandler { get; set; } = new GroupedDropHandler();
    public ObservableCollection<GroupedItem> GroupedCollection { get; set; } = new ObservableCollection<GroupedItem>();

    public ObservableCollection<DataGridRowModel> DataGridCollection1 { get; set; } = new ObservableCollection<DataGridRowModel>();
    public ObservableCollection<DataGridRowModel> DataGridCollection2 { get; set; } = new ObservableCollection<DataGridRowModel>();

    public ObservableCollection<TabItemModel> TabItemCollection1 { get; set; } = new ObservableCollection<TabItemModel>();
    public ObservableCollection<TabItemModel> TabItemCollection2 { get; set; } = new ObservableCollection<TabItemModel>();

    public TextBoxCustomDropHandler TextBoxCustomDropHandler { get; set; } = new TextBoxCustomDropHandler();
  }
}