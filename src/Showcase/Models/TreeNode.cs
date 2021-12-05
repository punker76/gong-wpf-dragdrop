using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Showcase.WPF.DragDrop.Models
{
    public class TreeNode : INotifyPropertyChanged, ICloneable
    {
        private string _caption;
        private ObservableCollection<TreeNode> _children;
        private bool _isCloned;
        private bool _isExpanded;

        public TreeNode(string caption)
        {
            this.Caption = caption;
            this.Children = new ObservableCollection<TreeNode>();
        }

        public string Caption
        {
            get => this._caption;
            set
            {
                if (value == this._caption) return;
                this._caption = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<TreeNode> Children
        {
            get => this._children;
            set
            {
                if (Equals(value, this._children)) return;
                this._children = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsCloned
        {
            get => this._isCloned;
            set
            {
                if (value == this._isCloned) return;
                this._isCloned = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => this._isExpanded;
            set
            {
                if (value == this._isExpanded) return;
                this._isExpanded = value;
                this.OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return this.Caption;
        }

        public object Clone()
        {
            var treeNode = new TreeNode(this.Caption) { IsCloned = true };
            foreach (var child in this.Children)
            {
                treeNode.Children.Add((TreeNode)child.Clone());
            }
            return treeNode;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}