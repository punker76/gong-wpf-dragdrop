#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#else
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#endif

namespace DefaultsExample
{
  public class WPFDataGrid : DataGrid
  {
  }

  public class WPFDataGridTextColumn : DataGridTextColumn
  {
  }
}