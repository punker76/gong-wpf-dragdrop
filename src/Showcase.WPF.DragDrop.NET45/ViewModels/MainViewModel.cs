using System.Diagnostics;
using System.Windows.Input;
using Showcase.WPF.DragDrop.Models;

namespace Showcase.WPF.DragDrop.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    private SampleData _data;
    private ICommand _openIssueCommand;

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
      this.OpenIssueCommand = new SimpleCommand(issue => { Process.Start($"https://github.com/punker76/gong-wpf-dragdrop/issues/{issue}"); });
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

    public ICommand OpenIssueCommand
    {
      get { return _openIssueCommand; }
      set
      {
        if (Equals(value, _openIssueCommand)) return;
        _openIssueCommand = value;
        OnPropertyChanged();
      }
    }
  }
}