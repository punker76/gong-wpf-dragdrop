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
            get => this._caption;
            set
            {
                if (value == this._caption) return;
                this._caption = value;
                this.OnPropertyChanged();
            }
        }

        public string Group

        {
            get => this._group;
            set
            {
                if (value == this._group)
                {
                    return;
                }

                this._group = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}