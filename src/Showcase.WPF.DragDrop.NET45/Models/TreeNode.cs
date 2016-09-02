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
      get { return _caption; }
      set
      {
        if (value == _caption) return;
        _caption = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<TreeNode> Children
    {
      get { return _children; }
      set
      {
        if (Equals(value, _children)) return;
        _children = value;
        OnPropertyChanged();
      }
    }

    public bool IsCloned
    {
      get { return _isCloned; }
      set
      {
        if (value == _isCloned) return;
        _isCloned = value;
        OnPropertyChanged();
      }
    }

    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (value == _isExpanded) return;
        _isExpanded = value;
        OnPropertyChanged();
      }
    }

    public override string ToString()
    {
      return this.Caption;
    }

    public object Clone()
    {
      var treeNode = new TreeNode(this.Caption) {IsCloned = true};
      foreach (var child in this.Children)
      {
        treeNode.Children.Add((TreeNode) child.Clone());
      }
      return treeNode;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}