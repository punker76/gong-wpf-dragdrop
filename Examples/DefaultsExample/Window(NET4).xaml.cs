using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace DefaultsExample
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      this.InitializeComponent();
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      MessageBox.Show(this, "Got the double click.");
    }

    private void FilterCheckBox_Clicked(object sender, RoutedEventArgs e)
    {
      var cv = (CollectionView)CollectionViewSource.GetDefaultView(SlavesList.ItemsSource);
      cv.Filter = ThreeFilter; //setting each time, since the Slaves list can be reset
      cv.Refresh();
    }

    private bool ThreeFilter(object item)
    {
      if (FilterCheck.IsChecked == true)
        return (item as SlaveDataModel).Number % 3 == 0;
      return true;
    }
  }
}