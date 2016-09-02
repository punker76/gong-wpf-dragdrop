using System.Windows;
using Showcase.WPF.DragDrop.ViewModels;

namespace Showcase.WPF.DragDrop
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = new MainViewModel();
    }
  }
}
