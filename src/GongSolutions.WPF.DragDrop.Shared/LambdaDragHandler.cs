namespace GongSolutions.WPF.DragDrop.Shared
{
    using System;
    using System.Windows;
    using GongSolutions.Wpf.DragDrop;

    /// <summary>
    /// This static class provides a factory method <see cref="Create"/> for creating stateless instances of <see cref="IDragSource"/> using <see cref="Action"/>s, <see cref="Action{T}"/>s, <see cref="Action{T1, T2}"/>s and <see cref="Func{T,TResult}"/>s.
    /// That way it isn't required to implement a whole class.
    /// </summary>
    public static class LambdaDragHandler
    {
        /// <summary>
        /// Creates an instance of <see cref="IDragSource"/> using the given <see cref="Action"/>s, <see cref="Action{T}"/>s, <see cref="Action{T1, T2}"/>s and <see cref="Func{T,TResult}"/>s as internal implementation.
        /// </summary>
        /// <param name="startDrag">The returned instance of <see cref="IDragSource"/> will use this <see cref="Action{T}"/> as inner implementation for the <see cref="IDragSource.StartDrag"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDragSource.StartDrag"/> call.</param>
        /// <param name="canStartDrag">The returned instance of <see cref="IDragSource"/> will use this <see cref="Func{T,TResult}"/> as inner implementation for the <see cref="IDragSource.CanStartDrag"/> function.
        /// If this parameter is null the function will always return true.</param>
        /// <param name="dropped">The returned instance of <see cref="IDragSource"/> will use this <see cref="Action{T}"/> as inner implementation for the <see cref="IDragSource.Dropped"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDragSource.StartDrag"/> call.</param>
        /// <param name="dragDropOperationFinished">The returned instance of <see cref="IDragSource"/> will use this <see cref="Action{T1, T2}"/> as inner implementation for the <see cref="IDragSource.DragDropOperationFinished"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDragSource.DragDropOperationFinished"/> call.</param>
        /// <param name="dragCanceled">The returned instance of <see cref="IDragSource"/> will use this <see cref="Action"/> as inner implementation for the <see cref="IDragSource.DragCancelled"/> method.
        /// If this parameter is null nothing will happen on the <see cref="IDragSource.DragCancelled"/> call.</param>
        /// <param name="tryCatchOccurredException">The returned instance of <see cref="IDragSource"/> will use this <see cref="Func{T,TResult}"/> as inner implementation for the <see cref="IDragSource.TryCatchOccurredException"/> function.
        /// If this parameter is null the function will always return false.</param>
        /// <returns>An instance of <see cref="IDropTarget"/> using the given <see cref="Action{T}"/>s, <see cref="Action{T1, T2}"/>s and <see cref="Func{T,TResult}"/>s as internal implementation.</returns>
        public static IDragSource Create(
            Action<IDragInfo> startDrag = null,
            Func<IDragInfo, bool> canStartDrag = null,
            Action<IDropInfo> dropped = null,
            Action<DragDropEffects, IDragInfo> dragDropOperationFinished = null,
            Action dragCanceled = null,
            Func<Exception, bool> tryCatchOccurredException = null)
        {
            return new LambdaDragHandlerInner(
                startDrag, 
                canStartDrag, 
                dropped, 
                dragDropOperationFinished, 
                dragCanceled, 
                tryCatchOccurredException);
        }

        private class LambdaDragHandlerInner : IDragSource
        {
            private readonly Action<IDragInfo> startDrag;
            private readonly Func<IDragInfo, bool> canStartDrag;
            private readonly Action<IDropInfo> dropped;
            private readonly Action<DragDropEffects, IDragInfo> dragDropOperationFinished;
            private readonly Action dragCanceled;
            private readonly Func<Exception, bool> tryCatchOccurredException;

            public LambdaDragHandlerInner(
                Action<IDragInfo> startDrag = null,
                Func<IDragInfo, bool> canStartDrag = null,
                Action<IDropInfo> dropped = null,
                Action<DragDropEffects, IDragInfo> dragDropOperationFinished = null,
                Action dragCanceled = null,
                Func<Exception, bool> tryCatchOccurredException = null)
            {
                this.startDrag = startDrag;
                this.canStartDrag = canStartDrag;
                this.dropped = dropped;
                this.dragDropOperationFinished = dragDropOperationFinished;
                this.dragCanceled = dragCanceled;
                this.tryCatchOccurredException = tryCatchOccurredException;
            }

            public void StartDrag(IDragInfo dragInfo)
            {
                this.startDrag?.Invoke(dragInfo);
            }

            public bool CanStartDrag(IDragInfo dragInfo)
            {
                return this.canStartDrag?.Invoke(dragInfo) ?? true;
            }

            public void Dropped(IDropInfo dropInfo)
            {
                this.dropped?.Invoke(dropInfo);
            }

            public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
            {
                this.dragDropOperationFinished?.Invoke(operationResult, dragInfo);
            }

            public void DragCancelled()
            {
                this.dragCanceled?.Invoke();
            }

            public bool TryCatchOccurredException(Exception exception)
            {
                return this.tryCatchOccurredException?.Invoke(exception) ?? false;
            }
        }
    }
}
