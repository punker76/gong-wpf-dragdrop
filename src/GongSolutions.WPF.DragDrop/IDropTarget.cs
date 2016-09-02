using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Interface implemented by Drop Handlers.
  /// </summary>
  public interface IDropTarget
  {
    /// <summary>
    /// Updates the current drag state.
    /// </summary>
    /// 
    /// <param name="dropInfo">
    ///   Information about the drag.
    /// </param>
    /// 
    /// <remarks>
    /// To allow a drop at the current drag position, the <see cref="DropInfo.Effects"/> property on 
    /// <paramref name="dropInfo"/> should be set to a value other than <see cref="DragDropEffects.None"/>
    /// and <see cref="DropInfo.Data"/> should be set to a non-null value.
    /// </remarks>
    void DragOver(IDropInfo dropInfo);

    /// <summary>
    /// Performs a drop.
    /// </summary>
    /// 
    /// <param name="dropInfo">
    ///   Information about the drop.
    /// </param>
    void Drop(IDropInfo dropInfo);
  }
}