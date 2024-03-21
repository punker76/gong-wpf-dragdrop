namespace Showcase.WPF.DragDrop.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class DestinationModel : INotifyPropertyChanged
{
    public string Name { get; set; }

    private int count;

    public int Count
    {
        get => this.count;
        set
        {
            if (value == this.count) return;
            this.count = value;
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