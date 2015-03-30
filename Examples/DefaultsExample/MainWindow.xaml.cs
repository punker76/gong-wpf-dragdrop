using System;
using System.Collections.Generic;
using System.Diagnostics;
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
  /// Interaction logic for Window.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      MessageBox.Show(this, "Got the double click.");
    }
    
    private void FilterCheckBox_Clicked(object sender, RoutedEventArgs e)
    {
        var coll = ((Data)this.DataContext).Collection1;
        var collView = CollectionViewSource.GetDefaultView(coll);
        collView.Filter += o => {
                               if (!FilterCheck.IsChecked.GetValueOrDefault()) {
                                   return true;
                               }
                               var str = (string) o;
                               Debug.Assert(str.StartsWith("Item"));
                               var number = Int32.Parse(str.Substring(4));
                               return (number & 0x01) == 0;
                           };
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      MessageBox.Show(this, "Got the click.");
    }
  }
}