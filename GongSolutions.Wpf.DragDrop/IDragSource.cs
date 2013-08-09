using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Interface implemented by Drag Handlers.
  /// </summary>
  public interface IDragSource
  {
    /// <summary>
    /// Queries whether a drag can be started.
    /// </summary>
    /// 
    /// <param name="dragInfo">
    /// Information about the drag.
    /// </param>
    /// 
    /// <remarks>
    /// To allow a drag to be started, the <see cref="DragInfo.Effects"/> property on <paramref name="dragInfo"/> 
    /// should be set to a value other than <see cref="DragDropEffects.None"/>. 
    /// </remarks>
    void StartDrag(IDragInfo dragInfo);

    /// <summary>
    /// Notifies the drag handler that a drop has occurred.
    /// </summary>
    /// 
    /// <param name="dropInfo">
    ///   Information about the drop.
    /// </param>
    void Dropped(IDropInfo dropInfo);
  }
}