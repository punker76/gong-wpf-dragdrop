using Showcase.WPF.DragDrop.Models;

namespace Showcase.WPF.DragDrop.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    private SampleData _data;

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel()
    {
      ////if (IsInDesignMode)
      ////{
      ////    // Code runs in Blend --> create design time data.
      ////}
      ////else
      ////{
      ////    // Code runs "for real"
      ////}

      this.Data = new SampleData();
    }

    public SampleData Data
    {
      get { return _data; }
      set
      {
        if (Equals(value, _data)) return;
        _data = value;
        OnPropertyChanged();
      }
    }
  }
}