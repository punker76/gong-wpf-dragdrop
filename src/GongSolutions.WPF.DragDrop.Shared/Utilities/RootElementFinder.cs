using System.Windows;
using System.Windows.Controls;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  public static class RootElementFinder
  {
    public static UIElement FindRoot(DependencyObject visual)
    {
      var parentWindow = Window.GetWindow(visual);
      var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
      if (rootElement == null)
      {
        if (Application.Current != null && Application.Current.MainWindow != null)
        {
          rootElement = Application.Current.MainWindow.Content as UIElement;
        }
        if (rootElement == null)
        {
          rootElement = visual.GetVisualAncestor<Page>() ?? visual.GetVisualAncestor<UserControl>() as UIElement;
        }
      }
      //      i don't want the fu... windows forms reference
      //      if (rootElement == null) {
      //          var elementHost = m_DragInfo.VisualSource.GetVisualAncestor<ElementHost>();
      //          rootElement = elementHost != null ? elementHost.Child : null;
      //      }
      return rootElement;
    }
  }
}