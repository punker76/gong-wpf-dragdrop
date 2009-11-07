using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DefaultsExample
{
    class Data
    {
        public Data()
        {
            Collection1 = new ObservableCollection<string>();
            Collection2 = new ObservableCollection<string>();

            for (int n = 0; n < 100; ++n)
            {
                Collection1.Add("Item " + n);
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
        public ObservableCollection<TreeNode> TreeCollection { get; private set; }
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
}
