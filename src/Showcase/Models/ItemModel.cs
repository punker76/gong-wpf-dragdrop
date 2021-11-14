using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Showcase.WPF.DragDrop.ViewModels;

namespace Showcase.WPF.DragDrop.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        private double _bindableDoubleValue;
        private string _selectedSubItem;
        private bool _isChecked;

        public ItemModel()
        {
            this.BindableDoubleValue = Faker.RandomNumber.Next(0, 100);
            for (int i = 0; i < Faker.RandomNumber.Next(2, 20); i++)
            {
                this.SubItemCollection.Add(new SubItemModel($"Sub item {i}"));
            }
        }

        public ItemModel(int itemIndex)
            : this()
        {
            this.Index = itemIndex;
            this.Caption = $"Item {itemIndex}";
        }

        public int Index { get; set; }

        public string Caption { get; set; }

        public ObservableCollection<SubItemModel> SubItemCollection { get; set; } = new ObservableCollection<SubItemModel>();

        public string SelectedSubItem
        {
            get => this._selectedSubItem;
            set
            {
                if (value == this._selectedSubItem) return;
                this._selectedSubItem = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get => this._isChecked;
            set
            {
                if (value == this._isChecked) return;
                this._isChecked = value;
                this.OnPropertyChanged();
            }
        }

        public double BindableDoubleValue
        {
            get => this._bindableDoubleValue;
            set
            {
                if (value.Equals(this._bindableDoubleValue)) return;
                this._bindableDoubleValue = value;
                this.OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return this.Caption;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SubItemModel : INotifyPropertyChanged
    {
        private string _bindableValue;
        private bool _bindableOptionA;
        private bool _bindableOptionB;

        public SubItemModel(string caption)
        {
            this.Caption = caption;
            this.ButtonTestCommand = new SimpleCommand(o => { this.BindableValue = $"Button clicked at {DateTime.UtcNow.ToLocalTime()}"; });
        }

        public string Caption { get; set; }

        public ICommand ButtonTestCommand { get; set; }

        public string BindableValue
        {
            get => this._bindableValue;
            set
            {
                if (value == this._bindableValue) return;
                this._bindableValue = value;
                this.OnPropertyChanged();
            }
        }

        public bool BindableOptionA
        {
            get => this._bindableOptionA;
            set
            {
                if (value == this._bindableOptionA) return;
                this._bindableOptionA = value;
                this.OnPropertyChanged();
            }
        }

        public bool BindableOptionB
        {
            get => this._bindableOptionB;
            set
            {
                if (value == this._bindableOptionB) return;
                this._bindableOptionB = value;
                this.OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return this.Caption;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class SerializableItemModel
    {
        public SerializableItemModel(int itemIndex)
        {
            this.Index = itemIndex;
            this.Caption = $"{itemIndex}. Item";
        }

        public int Index { get; set; }

        public string Caption { get; set; }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}