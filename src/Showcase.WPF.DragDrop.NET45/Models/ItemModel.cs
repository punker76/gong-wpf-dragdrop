using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Showcase.WPF.DragDrop.Models
{
  public class ItemModel : INotifyPropertyChanged
  {
    private double _bindableDoubleValue;

    public ItemModel(int itemIndex)
    {
      this.Caption = $"Item {itemIndex}";
      this.BindableDoubleValue = Faker.RandomNumber.Next(0, 100);
    }

    public string Caption { get; set; }

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