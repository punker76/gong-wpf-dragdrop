using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Showcase.WPF.DragDrop.Models
{
  public class ItemModel : INotifyPropertyChanged
  {
    private double _bindableDoubleValue;
    private string _selectedSubItem;

    public ItemModel()
    {
      this.BindableDoubleValue = Faker.RandomNumber.Next(0, 100);
      for (int i = 0; i < Faker.RandomNumber.Next(2, 20); i++)
      {
        SubItemCollection.Add($"Sub item {i}");
      }
    }

    public ItemModel(int itemIndex) : this()
    {
      this.Index = itemIndex;
      this.Caption = $"Item {itemIndex}";
    }

    public int Index { get; set; }
    public string Caption { get; set; }

    public ObservableCollection<string> SubItemCollection { get; set; } = new ObservableCollection<string>();

    public string SelectedSubItem
    {
      get { return _selectedSubItem; }
      set
      {
        if (value == _selectedSubItem) return;
        _selectedSubItem = value;
        OnPropertyChanged();
      }
    }

    public double BindableDoubleValue
    {
      get { return _bindableDoubleValue; }
      set
      {
        if (value.Equals(_bindableDoubleValue)) return;
        _bindableDoubleValue = value;
        OnPropertyChanged();
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
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}