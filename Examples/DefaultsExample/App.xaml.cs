using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace NorthwindExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
#if NET35
            this.StartupUri = new Uri("Window(NET35).xaml", UriKind.RelativeOrAbsolute);
#else
            this.StartupUri = new Uri("Window(NET4).xaml", UriKind.RelativeOrAbsolute);
#endif
        }
    }
}