using System;
using System.Collections.ObjectModel;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace DefaultsExample
{
  public class CustomDropHandlerForIssue85 : IDropTarget
  {
    public void DragOver(IDropInfo dropInfo)
    {
      dropInfo.Effects = DragDropEffects.Copy;
    }

    public void Drop(IDropInfo dropInfo)
    {
      MessageBox.Show("He, now it works :-D");
      ((TextBox)dropInfo.VisualTarget).Text = dropInfo.Data != null ? dropInfo.Data.ToString() : string.Empty;
    }
  }

  public class CustomDragHandlerForIssue84 : IDragSource
  {
    public virtual void StartDrag(IDragInfo dragInfo)
    {
      // nothing special here, use the default way
      DragDrop.DefaultDragHandler.StartDrag(dragInfo);
    }

    public bool CanStartDrag(IDragInfo dragInfo)
    {
      // so here is the magic
      if (dragInfo != null) {
        if ((dragInfo.SourceIndex % 2) == 0) {
          return false;
        }
      }
      return true;
    }

    public virtual void Dropped(IDropInfo dropInfo)
    {
    }

    public virtual void DragCancelled()
    {
    }

    public bool TryCatchOccurredException(Exception exception)
    {
      return false;
    }
  }

  internal class Data : IDropTarget
  {
    public Data()
    {
      this.Collection1 = new ObservableCollection<string>();
      this.Collection2 = new ObservableCollection<string>();
      this.Collection3 = new ObservableCollection<string>();
      this.Collection4 = new ObservableCollection<string>();
      this.CustomCollection1 = new ObservableCollection<CustomDataModel>();
      this.CustomCollection2 = new ObservableCollection<CustomDataModel>();
      this.ClonableCollection1 = new ObservableCollection<ClonableDataModel>();
      this.ClonableCollection2 = new ObservableCollection<ClonableDataModel>();

      this.CustomDropHandler = new CustomDropHandlerForIssue85();
      this.CustomDragHandler = new CustomDragHandlerForIssue84();

      for (var n = 0; n < 100; ++n) {
        this.Collection1.Add("Item " + n);
        this.CustomCollection1.Add(new CustomDataModel { Name = "Custom Item " + n });
        this.ClonableCollection1.Add(new ClonableDataModel("Clonable Item " + n ));
      }

      for (var n = 0; n < 4; ++n) {
        this.Collection4.Add("Content " + n);
      }

      this.GroupedCollection = new ObservableCollection<GroupedItem>();
      for (var g = 0; g < 4; ++g) {
        for (var i = 0; i < 2; ++i) {
          this.GroupedCollection.Add(new GroupedItem("Group " + g, "Group " + g + " Item " + i));
        }
      }

      var root1 = new TreeNode("Root 1");
      for (var n = 0; n < 10; ++n) {
        root1.Children.Add(new TreeNode("Item " + n));
      }

      var root2 = new TreeNode("Root 2");
      for (var n = 0; n < 4; ++n) {
        root2.Children.Add(new TreeNode("Item " + (n + 10)));
      }

      this.TreeCollection = new ObservableCollection<TreeNode> {
                                                                 root1, root2
                                                               };
      this.TreeCollection2 = new ObservableCollection<TreeNode>();
    }

    public ObservableCollection<string> Collection1 { get; private set; }
    public ObservableCollection<string> Collection2 { get; private set; }
    public ObservableCollection<string> Collection3 { get; private set; }
    public ObservableCollection<string> Collection4 { get; private set; }
    public ObservableCollection<CustomDataModel> CustomCollection1 { get; private set; }
    public ObservableCollection<CustomDataModel> CustomCollection2 { get; private set; }
    public ObservableCollection<ClonableDataModel> ClonableCollection1 { get; private set; }
    public ObservableCollection<ClonableDataModel> ClonableCollection2 { get; private set; }
    public ObservableCollection<GroupedItem> GroupedCollection { get; private set; }
    public ObservableCollection<TreeNode> TreeCollection { get; private set; }
    public ObservableCollection<TreeNode> TreeCollection2 { get; private set; }

    public CustomDropHandlerForIssue85 CustomDropHandler { get; private set; }
    public CustomDragHandlerForIssue84 CustomDragHandler { get; private set; }

    //
    // The drop handler is only used for the grouping example.
    //

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
      DragDrop.DefaultDropHandler.DragOver(dropInfo);
      if ((dropInfo.Data is CustomDataModel) || dropInfo.TargetGroup == null) {
        dropInfo.Effects = System.Windows.DragDropEffects.None;
      }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
      // I know this example is called DefaultsExample, but the default handlers don't know how
      // to set an item's group. You need to explicitly set the group on the dropped item like this.
      DragDrop.DefaultDropHandler.Drop(dropInfo);
      var data = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<GroupedItem>().ToList();
      foreach (var groupedItem in data) {
        groupedItem.Group = dropInfo.TargetGroup.Name.ToString();
      }

      // Changing group data at runtime isn't handled well: force a refresh on the collection view.
      if (dropInfo.TargetCollection is ICollectionView) {
        ((ICollectionView)dropInfo.TargetCollection).Refresh();
      }
    }
  }

  internal class CustomDataModel : IDropTarget
  {
    public string Name { get; set; }

    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.DragInfo.Data is CustomDataModel) {
        dropInfo.Effects = DragDropEffects.Copy;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

        // test for handled
        dropInfo.NotHandled = true; // now the DefaultDropHandler should work
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.DragInfo.Data is CustomDataModel) {
        dropInfo.NotHandled = true; // now the DefaultDropHandler should work
      }
    }
  }

  internal class ClonableDataModel : ICloneable, INotifyPropertyChanged
  {
    public ClonableDataModel(string name)
    {
      Name = name;
    }

    private string _name;
    public string Name
    {
      get { return this._name; }
      set
      {
        if (this._name != value)
        {
          this._name = value;
          this.OnPropertyChanged("Name");
        }
      }
    }

    public object Clone()
    {
      return new ClonableDataModel(Name + ", now cloned");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null) { handler(this, new PropertyChangedEventArgs(propertyName)); }
    }
  }

  internal class TreeNode : ICloneable
  {
    public TreeNode(string caption)
    {
      this.Caption = caption;
      this.Children = new ObservableCollection<TreeNode>();
    }

    public string Caption { get; private set; }
    public ObservableCollection<TreeNode> Children { get; private set; }

    public override string ToString()
    {
      return this.Caption;
    }

    public object Clone()
    {
      var treeNode = new TreeNode(this.Caption);
      foreach (var child in this.Children)
      {
        treeNode.Children.Add((TreeNode) child.Clone());
      }
      return treeNode;
    }
  }

  internal class GroupedItem : INotifyPropertyChanged
  {
    public GroupedItem(string group, string caption)
    {
      this.Caption = caption;
      this.m_Group = group;
    }

    public override string ToString()
    {
      return this.m_Group + " " + this.Caption;
    }

    public string Caption { get; private set; }

    public string Group
    {
      get { return this.m_Group; }
      set
      {
        if (this.m_Group != value) {
          this.m_Group = value;
          this.OnPropertyChanged("Group");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name)
    {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    private string m_Group;
  }

  internal class ItemViewModel
  {
    public int Index { get; set; }
  }
}