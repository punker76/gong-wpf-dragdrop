namespace GongSolutions.Wpf.DragDrop;

using System;
using System.Collections.Generic;
using System.Windows;

/// <summary>
/// Helper methods to assist with drop hints, used through <see cref="DragDrop.UseDropTargetHintProperty"/>.
/// </summary>
internal static class DropHintHelpers
{
    private static readonly List<DropTargetHintWrapper> _dropTargetHintWrappers = new();

    /// <summary>
    /// Add reference to drop target so we can show hint when drag operation start.
    /// </summary>
    /// <param name="dropTarget"></param>
    public static void AddDropHintTarget(UIElement dropTarget)
    {
        _dropTargetHintWrappers.Add(new DropTargetHintWrapper(dropTarget));
        CleanDeadwood();
    }

    /// <summary>
    /// Remove reference to drop target, to avoid memory leaks.
    /// </summary>
    /// <param name="dropTarget"></param>
    public static void RemoveDropHintTarget(UIElement dropTarget)
    {
        _dropTargetHintWrappers.RemoveAll(m => m.Target == dropTarget);
        CleanDeadwood();
    }

    /// <summary>
    /// Show all available drop hints.
    /// </summary>
    /// <param name="dragInfo"></param>
    public static void ShowDropHintAdorners(DragInfo dragInfo)
    {
        CleanDeadwood();
        var visibleTargets = GetVisibleTargets();
        foreach (var wrapper in visibleTargets)
        {
            var sender = wrapper.Target;

            var handler = DragDrop.TryGetDropHandler(null, sender);
            if (handler != null)
            {
                var dropHintInfo = new DropHintInfo(dragInfo);
                handler.DropHint(dropHintInfo);
                UpdateHint(dropHintInfo, wrapper);
            }
        }
    }

    /// <summary>
    /// Update drop hint for the current element.
    /// </summary>
    /// <param name="dropHandler"></param>
    /// <param name="dragInfo"></param>
    /// <param name="dropInfo"></param>
    /// <param name="sender"></param>
    public static void OnDropHintLeave(IDropTarget dropHandler, DragInfo dragInfo, IDropInfo dropInfo, object sender)
    {
        var wrapper = _dropTargetHintWrappers.Find(m => m.Target == sender);
        if (wrapper != null)
        {
            var dropHintInfo = new DropHintInfo(dragInfo, dropInfo);
            dropHandler.DropHint(dropHintInfo);
            UpdateHint(dropHintInfo, wrapper);
        }
    }

    /// <summary>
    /// Update drop hint for the current element.
    /// </summary>
    /// <param name="dropHandler"></param>
    /// <param name="dragInfo"></param>
    /// <param name="dropInfo"></param>
    /// <param name="sender"></param>
    public static void OnDropHintEnter(IDropTarget dropHandler, DragInfo dragInfo, IDropInfo dropInfo, object sender)
    {
        var wrapper = _dropTargetHintWrappers.Find(m => m.Target == sender);
        if (wrapper != null)
        {
            var dropHintInfo = new DropHintInfo(dragInfo, dropInfo);
            dropHandler.DropHintOver(dropHintInfo);
            UpdateHint(dropHintInfo, wrapper);
        }
    }

    private static void UpdateHint(IDropHintInfo dropHintInfo, DropTargetHintWrapper wrapper)
    {
        var dataTemplate = DragDrop.GetDropHintDataTemplate(wrapper.Target);
        if (dropHintInfo.DropTargetHintAdorner == null)
        {
            wrapper.DropTargetHintAdorner = null;
        }

        if (dropHintInfo.DropTargetHintAdorner != null && dropHintInfo.DropTargetHintAdorner.IsAssignableTo(typeof(DropTargetHintAdorner)))
        {
            wrapper.DropTargetHintAdorner = DropTargetHintAdorner.CreateHintAdorner(dropHintInfo.DropTargetHintAdorner, wrapper.Target, dropHintInfo, dataTemplate);
        }
    }

    /// <summary>
    /// Helper method for getting available hint drop targets.
    /// </summary>
    /// <returns></returns>
    private static List<DropTargetHintWrapper> GetVisibleTargets()
    {
        return _dropTargetHintWrappers.FindAll(m => m.Target?.IsVisible == true && DragDrop.GetIsDropTarget(m.Target));
    }

    /// <summary>
    /// Clean deadwood in case we are holding on to references to dead objects.
    /// </summary>
    private static void CleanDeadwood()
    {
        _dropTargetHintWrappers.RemoveAll((m => !m.IsAlive));
    }

    public static void HideDropHintAdorners()
    {
        CleanDeadwood();
        foreach (var target in _dropTargetHintWrappers)
        {
            target.DropTargetHintAdorner = null;
        }
    }
}

/// <summary>
/// Wrapper of the <see cref="UIElement"/> so we only have weak references to the drop targets
/// to avoid memory leaks.
/// </summary>
internal sealed class DropTargetHintWrapper : IDisposable
{
    private readonly WeakReference<UIElement> _dropTarget;
    private DropTargetHintAdorner dropTargetHintAdorner;

    public DropTargetHintWrapper(UIElement dropTarget)
    {
        _dropTarget = new WeakReference<UIElement>(dropTarget);
    }

    public UIElement Target => _dropTarget.TryGetTarget(out var target) ? target : null;

    /// <summary>
    /// Property indicating if the weak reference is still alive, or should be disposed of.
    /// </summary>
    public bool IsAlive => _dropTarget.TryGetTarget(out _);

    public DropTargetHintAdorner DropTargetHintAdorner
    {
        get => this.dropTargetHintAdorner;
        set
        {
            this.dropTargetHintAdorner?.Detatch();
            this.dropTargetHintAdorner = value;
        }
    }

    public void Dispose()
    {
        this.DropTargetHintAdorner = null;
    }
}