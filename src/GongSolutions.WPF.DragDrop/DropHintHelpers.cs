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
    /// Clean deadwood in case we are holding on to references to dead objects.
    /// </summary>
    private static void CleanDeadwood()
    {
        _dropTargetHintWrappers.RemoveAll((m => !m.IsAlive));
    }
}

/// <summary>
/// Wrapper of the <see cref="UIElement"/> so we only have weak references to the drop targets
/// to avoid memory leaks.
/// </summary>
internal class DropTargetHintWrapper
{
    private readonly WeakReference<UIElement> _dropTarget;

    public DropTargetHintWrapper(UIElement dropTarget)
    {
        _dropTarget = new WeakReference<UIElement>(dropTarget);
    }

    public UIElement Target => _dropTarget.TryGetTarget(out var target) ? target : null;

    /// <summary>
    /// Property indicating if the weak reference is still alive, or should be disposed of.
    /// </summary>
    public bool IsAlive => _dropTarget.TryGetTarget(out _);
}