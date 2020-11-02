using System.Windows;
using System.Windows.Controls;

namespace Showcase.WPF.DragDrop.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty
            = DependencyProperty.Register(nameof(Caption),
                                          typeof(string),
                                          typeof(SettingsView),
                                          new PropertyMetadata(default(string)));

        public string Caption
        {
            get => (string)this.GetValue(CaptionProperty);
            set => this.SetValue(CaptionProperty, value);
        }
    }
}