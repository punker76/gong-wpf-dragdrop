using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using GongSolutions.Wpf.DragDrop;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections;

namespace DefaultsExample
{
    internal class Data : IDropTarget
    {
        public Data()
        {
            this.Collection1 = new ObservableCollection<string>();
            this.Collection2 = new ObservableCollection<string>();

            for (var n = 0; n < 100; ++n)
            {
                this.Collection1.Add("Item " + n);
            }

            this.GroupedCollection = new ObservableCollection<GroupedItem>();
            for (var g = 0; g < 4; ++g)
            {
                for (var i = 0; i < 2; ++i)
                {
                    this.GroupedCollection.Add(new GroupedItem("Group " + g, "Group " + g + " Item " + i));
                }
            }

            var root1 = new TreeNode("Root 1");
            for (var n = 0; n < 10; ++n)
            {
                root1.Children.Add(new TreeNode("Item " + n));
            }

            var root2 = new TreeNode("Root 2");
            for (var n = 0; n < 4; ++n)
            {
                root2.Children.Add(new TreeNode("Item " + (n + 10)));
            }

            this.TreeCollection = new ObservableCollection<TreeNode> {
                                                                         root1, root2
                                                                     };
        }

        public ObservableCollection<string> Collection1 { get; private set; }
        public ObservableCollection<string> Collection2 { get; private set; }
        public ObservableCollection<GroupedItem> GroupedCollection { get; private set; }
        public ObservableCollection<TreeNode> TreeCollection { get; private set; }

        //
        // The drop handler is only used for the grouping example.
        //

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
            if (dropInfo.TargetGroup == null)
            {
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
            if (dropInfo.TargetCollection is ICollectionView)
            {
                ((ICollectionView)dropInfo.TargetCollection).Refresh();
            }
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
                if (this.m_Group != value)
                {
                    this.m_Group = value;
                    this.OnPropertyChanged("Group");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private string m_Group;
    }
}