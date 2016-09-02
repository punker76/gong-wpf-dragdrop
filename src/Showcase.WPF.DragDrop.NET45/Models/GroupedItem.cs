using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Showcase.WPF.DragDrop.Models
{
  public class GroupedItem : INotifyPropertyChanged
  {
    private string _caption;
    private string _group;

    public GroupedItem(int group, int item)
    {
      this.Caption = $"Item {item} from Group {group}";
      this._group = $"Group {group}";
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

    public string Group

    {
      get { return _group; }
      set
      {
        if (value == _group)
        {
          return;
        }
        _group = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}