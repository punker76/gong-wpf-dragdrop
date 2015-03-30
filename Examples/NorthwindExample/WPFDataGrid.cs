#if NET35
using Microsoft.Windows.Controls;

#else
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#endif

namespace NorthwindExample
{
  public class WPFDataGrid : DataGrid
  {
  }

  public class WPFDataGridTextColumn : DataGridTextColumn
  {
  }
}