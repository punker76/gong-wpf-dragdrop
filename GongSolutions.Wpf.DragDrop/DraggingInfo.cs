using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Data;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Holds information about a the target of a drag drop operation.
  /// </summary>
  /// 
  /// <remarks>
  /// The <see cref="DropInfo"/> class holds all of the framework's information about the current 
  /// target of a drag. It is used by <see cref="IDropTarget.DragOver"/> method to determine whether 
  /// the current drop target is valid, and by <see cref="IDropTarget.Drop"/> to perform the drop.
  /// </remarks>
  public class DraggingInfo : IDraggingInfo
  {


    /// <summary>
    /// Initializes a new instance of the DropInfo class.
    /// </summary>
    /// 
    /// <param name="sender">
    /// The sender of the drag event.
    /// </param>
    /// 
    /// <param name="e">
    /// The drag event.
    /// </param>
    /// 
    /// <param name="dragInfo">
    /// Information about the source of the drag, if the drag came from within the framework.
    /// </param>
    public DraggingInfo(DragInfo dragInfo, bool dropped)
    {
      this.DragInfo = dragInfo;
      this.AbsolutePosition = MouseUtilities.GetCursorPosition();
      this.Delta = AbsolutePosition - dragInfo.DragStartAbsolutePosition;
      this.Dropped = dropped;
    }
    
    /// <summary>
    /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
    /// if the drag came from within the framework.
    /// </summary>
    public IDragInfo DragInfo { get; private set; }

    

    /// <summary>
    /// Gets the mouse position
    /// </summary>
    public Point AbsolutePosition { get; }


    /// <summary>
    /// Gets the mouse delta from start
    /// </summary>
    public Vector Delta { get; }


    /// <summary>
    /// If this was dropped
    /// </summary>
    public bool Dropped { get; }
  }
    
}
