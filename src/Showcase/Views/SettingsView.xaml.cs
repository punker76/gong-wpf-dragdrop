using System.Windows;
using System.Windows.Controls;

namespace Showcase.WPF.DragDrop.Views
{
    using System.Windows.Input;

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

        public static readonly DependencyProperty ShowCopyKeyStateProperty
            = DependencyProperty.Register(nameof(ShowCopyKeyState),
                                          typeof(bool),
                                          typeof(SettingsView),
                                          new PropertyMetadata(default(bool)));

        public bool ShowCopyKeyState
        {
            get => (bool)this.GetValue(ShowCopyKeyStateProperty);
            set => this.SetValue(ShowCopyKeyStateProperty, value);
        }

        public static readonly DependencyProperty FilterCollectionCommandProperty
            = DependencyProperty.Register(nameof(FilterCollectionCommand),
                                          typeof(ICommand),
                                          typeof(SettingsView),
                                          new PropertyMetadata(default(ICommand), (o, args) => ((SettingsView)o).FilterCollectionCommandShown = args.NewValue != null));

        public ICommand FilterCollectionCommand
        {
            get => (ICommand)this.GetValue(FilterCollectionCommandProperty);
            set => this.SetValue(FilterCollectionCommandProperty, value);
        }

        private static readonly DependencyPropertyKey FilterCollectionCommandShownPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(FilterCollectionCommandShown),
                                                  typeof(bool),
                                                  typeof(SettingsView),
                                                  new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty FilterCollectionCommandShownProperty = FilterCollectionCommandShownPropertyKey.DependencyProperty;

        public bool FilterCollectionCommandShown
        {
            get => (bool)this.GetValue(FilterCollectionCommandShownProperty);
            protected set => this.SetValue(FilterCollectionCommandShownPropertyKey, value);
        }
    }
}