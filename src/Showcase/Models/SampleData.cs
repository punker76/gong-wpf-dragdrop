using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GongSolutions.Wpf.DragDrop;

namespace Showcase.WPF.DragDrop.Models
{
    using MahApps.Metro.IconPacks;

    public class SampleData
    {
        public SampleData()
        {
            for (var n = 0; n < 10_000; ++n)
            {
                this.SerializableCollection1.Add(new SerializableItemModel(n + 1));
                this.Collection1.Add(new ItemModel(n + 1));
                this.FilterCollection1.Add(new ItemModel(n + 1));
                this.ClonableCollection1.Add(new ClonableItemModel(n + 1));
                this.DataGridCollection1.Add(new DataGridRowModel());
            }

            for (var n = 0; n < 10; ++n)
            {
                this.Collection4.Add(new ItemModel() { Caption = $"Model {n + 1}" });
            }

            for (var g = 0; g < 4; ++g)
            {
                for (var i = 0; i < ((g % 2) == 0 ? 4 : 2); ++i)
                {
                    this.GroupedCollection.Add(new GroupedItem(g, i));
                }
            }

            this.GroupedItemsCollectionViewSource = CollectionViewSource.GetDefaultView(this.GroupedCollection);
            this.GroupedItemsCollectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = nameof(GroupedItem.Group) });

            for (int r = 1; r <= 6; r++)
            {
                var root = new TreeNode($"Root {r}");
                var folder = new TreeNode($"Folder {r}") { Icon = PackIconMaterialKind.Folder };
                for (var i = 0; i < ((r % 2) == 0 ? 8 : 3); ++i)
                {
                    root.Children.Add(new TreeNode($"Item {i + 10 * r}"));
                    folder.Children.Add(new TreeNode($"File {i + 10 * r}") { Icon = PackIconMaterialKind.File });
                }

                this.TreeCollection1.Add(root);
                this.TreeCollectionFiles.Add(folder);
                if (r == 2)
                {
                    root.IsExpanded = true;
                    folder.IsExpanded = true;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                this.TabItemCollection1.Add(new TabItemModel(i + 1));
            }

            this.TabItemCollection2.Add(new TabItemModel(1));
        }

        public ObservableCollection<SerializableItemModel> SerializableCollection1 { get; set; } = new ObservableCollection<SerializableItemModel>();

        public ObservableCollection<SerializableItemModel> SerializableCollection2 { get; set; } = new ObservableCollection<SerializableItemModel>();

        public SerializableDragHandler SerializableDragHandler { get; set; } = new SerializableDragHandler();

        public SerializableDropHandler SerializableDropHandler { get; set; } = new SerializableDropHandler();

        public ItemModelObservableCollection Collection1 { get; set; } = new ItemModelObservableCollection();

        public ObservableCollection<ItemModel> Collection2 { get; set; } = new ObservableCollection<ItemModel>();

        public ObservableCollection<ItemModel> Collection3 { get; set; } = new ObservableCollection<ItemModel>();

        public ObservableCollection<ItemModel> Collection4 { get; set; } = new ObservableCollection<ItemModel>();

        public ObservableCollection<ClonableItemModel> ClonableCollection1 { get; set; } = new ObservableCollection<ClonableItemModel>();

        public ObservableCollection<ClonableItemModel> ClonableCollection2 { get; set; } = new ObservableCollection<ClonableItemModel>();

        public ObservableCollection<ItemModel> FilterCollection1 { get; set; } = new ObservableCollection<ItemModel>();

        public ObservableCollection<ItemModel> FilterCollection2 { get; set; } = new ObservableCollection<ItemModel>();

        public ObservableCollection<TreeNode> TreeCollection1 { get; set; } = new ObservableCollection<TreeNode>();

        public ObservableCollection<TreeNode> TreeCollection2 { get; set; } = new ObservableCollection<TreeNode>();

        public ObservableCollection<TreeNode> TreeCollectionFiles { get; set; } = new ObservableCollection<TreeNode>();

        public FilesDropHandler FilesDropHandler { get; set; } = new FilesDropHandler();

        public GroupedDropHandler GroupedDropHandler { get; set; } = new GroupedDropHandler();

        public ObservableCollection<GroupedItem> GroupedCollection { get; set; } = new ObservableCollection<GroupedItem>();

        public ICollectionView GroupedItemsCollectionViewSource { get; }

        public ObservableCollection<DataGridRowModel> DataGridCollection1 { get; set; } = new ObservableCollection<DataGridRowModel>();

        public ObservableCollection<DataGridRowModel> DataGridCollection2 { get; set; } = new ObservableCollection<DataGridRowModel>();

        public ObservableCollection<TabItemModel> TabItemCollection1 { get; set; } = new ObservableCollection<TabItemModel>();

        public ObservableCollection<TabItemModel> TabItemCollection2 { get; set; } = new ObservableCollection<TabItemModel>();

        public TextBoxCustomDropHandler TextBoxCustomDropHandler { get; set; } = new TextBoxCustomDropHandler();

        public ListBoxCustomDropHandler ListBoxCustomDropHandler { get; set; } = new ListBoxCustomDropHandler();

        public IDropTarget NestedDropHandler { get; set; } = new NestedDropHandler();
    }
}