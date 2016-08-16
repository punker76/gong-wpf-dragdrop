using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GongSolutions.Wpf.DragDrop
{
  public interface IDraggingInfo
  {

    /// <summary>
    /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
    /// if the drag came from within the framework.
    /// </summary>
    IDragInfo DragInfo { get; }

    /// <summary>
    /// Gets the mouse position
    /// </summary>
    Point AbsolutePosition { get; }

    /// <summary>
    /// Gets the mouse delta from start
    /// </summary>
    Vector Delta { get; }

    /// <summary>
    /// If this was dropped
    /// </summary>
    bool Dropped { get; }
  }
}