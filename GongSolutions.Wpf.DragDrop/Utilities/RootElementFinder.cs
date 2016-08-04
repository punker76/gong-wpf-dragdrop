using System.Windows;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class RootElementFinder
    {
        public static UIElement FindRoot(DependencyObject d)
        {
            var parentWindow = Window.GetWindow(d);
            var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
            if (rootElement == null && Application.Current != null && Application.Current.MainWindow != null)
            {
                rootElement = (UIElement)Application.Current.MainWindow.Content;
            }

            return rootElement;
        }
    }
}