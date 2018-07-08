namespace GongSolutions.WPF.DragDrop.Shared
{
    using System;
    using GongSolutions.Wpf.DragDrop;

    /// <summary>
    /// This static class provides a factory method <see cref="Create"/> for creating stateless instances of <see cref="IDropTarget"/> using <see cref="Action{T}"/>s.
    /// That way it isn't required to implement a whole class.
    /// </summary>
    public static class LambdaDropHandler
    {
        /// <summary>
        /// Creates an instance of <see cref="IDropTarget"/> using the given <see cref="Action{T}"/>s as internal implementation.
        /// </summary>
        /// <param name="dragOver">The returned instance of <see cref="IDropTarget"/> will use this <see cref="Action{T}"/> as inner implementation for the <see cref="IDropTarget.DragOver"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDropTarget.DragOver"/> call.</param>
        /// <param name="drop">The returned instance of <see cref="IDropTarget"/> will use this <see cref="Action{T}"/> as inner implementation for the <see cref="IDropTarget.Drop"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDropTarget.Drop"/> call.</param>
        /// <returns>An instance of <see cref="IDropTarget"/> using the given Actions as internal implementation.</returns>
        public static IDropTarget Create(Action<IDropInfo> dragOver = null, Action<IDropInfo> drop = null)
        {
            return new LambdaDropHandlerInner(dragOver, drop);
        }
        
        private class LambdaDropHandlerInner : IDropTarget
        {
            private readonly Action<IDropInfo> dragOver;
            private readonly Action<IDropInfo> drop;

            public LambdaDropHandlerInner(Action<IDropInfo> dragOver = null, Action<IDropInfo> drop = null)
            {
                this.dragOver = dragOver;
                this.drop = drop;
            }

            public void DragOver(IDropInfo dropInfo)
            {
                this.dragOver?.Invoke(dropInfo);
            }

            public void Drop(IDropInfo dropInfo)
            {
                this.drop?.Invoke(dropInfo);
            }
        }
    }
}
