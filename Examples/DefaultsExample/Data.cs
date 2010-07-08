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
    class Data : IDropTarget
    {
        public Data()
        {
            Collection1 = new ObservableCollection<string>();
            Collection2 = new ObservableCollection<string>();

            for (int n = 0; n < 100; ++n)
            {
                Collection1.Add("Item " + n);
            }

            GroupedCollection = new ObservableCollection<GroupedItem>();
            for (int g = 0; g < 4; ++g)
            {
                for (int i = 0; i < 2; ++i)
                {
                    GroupedCollection.Add(new GroupedItem("Group " + g, "Group " + g + " Item " + i));
                }
            }

            TreeNode root1 = new TreeNode("Root 1");
            for (int n = 0; n < 10; ++n)
            {
                root1.Children.Add(new TreeNode("Item " + n));
            }

            TreeNode root2 = new TreeNode("Root 2");
            for (int n = 0; n < 4; ++n)
            {
                root2.Children.Add(new TreeNode("Item " + (n + 10)));
            }

            TreeCollection = new ObservableCollection<TreeNode>
            {
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
            if (dropInfo.TargetGroup == null) dropInfo.Effects = System.Windows.DragDropEffects.None;
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

    class TreeNode
    {
        public TreeNode(string caption)
        {
            Caption = caption;
            Children = new ObservableCollection<TreeNode>();
        }

        public string Caption { get; private set; }
        public ObservableCollection<TreeNode> Children { get; private set; }
    }

    class GroupedItem : INotifyPropertyChanged
    {
        public GroupedItem(string group, string caption)
        {
            Caption = caption;
            m_Group = group;
        }

        public override string ToString()
        {
            return m_Group + " " + Caption;
        }

        public string Caption { get; private set; }
        
        public string Group 
        {
            get { return m_Group; }
            set
            {
                if (m_Group != value)
                {
                    m_Group = value;
                    OnPropertyChanged("Group");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        string m_Group;
    }
}
