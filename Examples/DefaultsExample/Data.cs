﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections;
using GongSolutions.Wpf.DragDrop.Utilities;
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
      ((TextBox)dropInfo.VisualTarget).Text = (string)dropInfo.Data;
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
  }

  internal class Data : IDropTarget
  {
    public Data()
    {
      this.Collection1 = new ObservableCollection<string>();
      this.Collection2 = new ObservableCollection<string>();
      this.Collection3 = new ObservableCollection<string>();
      this.CustomCollection1 = new ObservableCollection<CustomDataModel>();
      this.CustomCollection2 = new ObservableCollection<CustomDataModel>();

      this.CustomDropHandler = new CustomDropHandlerForIssue85();
      this.CustomDragHandler = new CustomDragHandlerForIssue84();

      this.Masters = new ObservableCollection<MasterDataModel>();
      this.Masters.Add(new MasterDataModel("Fizz",3));
      this.Masters.Add(new MasterDataModel("Buzz", 5));
      this.Masters.Add(new MasterDataModel("FizzBuzz", 15));
      this.MasterDropHandler = new MasterDropHandler();

      for (var n = 0; n < 100; ++n) {
        this.Collection1.Add("Item " + n);
        this.CustomCollection1.Add(new CustomDataModel { Name = "Custom Item " + n });
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
    }

    public ObservableCollection<string> Collection1 { get; private set; }
    public ObservableCollection<string> Collection2 { get; private set; }
    public ObservableCollection<string> Collection3 { get; private set; }
    public ObservableCollection<CustomDataModel> CustomCollection1 { get; private set; }
    public ObservableCollection<CustomDataModel> CustomCollection2 { get; private set; }
    public ObservableCollection<GroupedItem> GroupedCollection { get; private set; }
    public ObservableCollection<TreeNode> TreeCollection { get; private set; }
    public ObservableCollection<MasterDataModel> Masters { get; private set; }

    public CustomDropHandlerForIssue85 CustomDropHandler { get; private set; }
    public CustomDragHandlerForIssue84 CustomDragHandler { get; private set; }
    public MasterDropHandler MasterDropHandler { get; private set; }

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
      ((GroupedItem)dropInfo.Data).Group = dropInfo.TargetGroup.Name.ToString();

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

  internal class SlaveDataModel
  {
    public string Name { get; set; }
    public int Number { get; set; }
    public string VarText { get; set; }
  }

  internal class MasterDataModel
  {
    public MasterDataModel(string n, int initialNumSlaves)
    {
      Name = n;
      Models = new ObservableCollection<SlaveDataModel>();

      string text = "";
      for (int i = 0; i < initialNumSlaves; i++) {
        text += n + " ";
        Models.Add(new SlaveDataModel() { Name = string.Format("Num {0}", i), Number = i, VarText = text });
      }
    }
    public string Name { get; set; }
    public ObservableCollection<SlaveDataModel> Models { get; set; }
  }

  internal class MasterDropHandler: DefaultDropHandler
  {
    public override void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data.GetType() == typeof(SlaveDataModel)) {
        dropInfo.Effects = DragDropEffects.Copy;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
      }
      else
        base.DragOver(dropInfo);
    }

    public override void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.Data.GetType() == typeof(SlaveDataModel)) {
        var holder = dropInfo.TargetItem as MasterDataModel;
        var dropped = dropInfo.Data as SlaveDataModel;
        holder.Models.Add(dropped);
      }
      else
        base.Drop(dropInfo);
    }
  }

  internal class TreeNode
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
}