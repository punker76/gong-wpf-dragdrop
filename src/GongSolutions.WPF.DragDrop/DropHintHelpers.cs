using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

namespace GongSolutions.Wpf.DragDrop
{
    /// <summary>
    /// Helper methods to assist with drop hints, used through <see cref="DragDrop.UseDropTargetHintProperty"/>.
    /// </summary>
    internal static class DropHintHelpers
    {
        private static readonly List<DropTargetHintWeakReference> _dropTargetHintReferences = new();

        /// <summary>
        /// Add reference to drop target so we can show hint when drag operation start.
        /// </summary>
        /// <param name="dropTarget"></param>
        public static void AddDropHintTarget(UIElement dropTarget)
        {
            _dropTargetHintReferences.Add(new DropTargetHintWeakReference(dropTarget));
            CleanDeadwood();
        }

        /// <summary>
        /// Remove reference to drop target.
        /// </summary>
        /// <param name="dropTarget"></param>
        public static void RemoveDropHintTarget(UIElement dropTarget)
        {
            _dropTargetHintReferences.RemoveAll(m => m.Target == dropTarget);
            CleanDeadwood();
        }

        /// <summary>
        /// Show all available drop hints.
        /// </summary>
        /// <param name="dragInfo"></param>
        public static void OnDragStart(IDragInfo dragInfo)
        {
            CleanDeadwood();
            var visibleTargets = GetVisibleTargets();
            foreach (var weakReference in visibleTargets)
            {
                var sender = weakReference.Target;

                var handler = DragDrop.TryGetDropHandler(null, sender);
                if (handler != null)
                {
                    var dropHintInfo = new DropHintInfo(dragInfo);
                    handler.DropHint(dropHintInfo);
                    UpdateHintAdorner(weakReference,
                                      dropHintInfo.DropTargetHintAdorner,
                                      new DropHintData(dropHintInfo.DropTargetHintState, dropHintInfo.DropHintText));
                }
            }
        }

        /// <summary>
        /// Clears all hint adorner from all drop targets when drag operation is finished.
        /// </summary>
        public static void OnDropFinished()
        {
            CleanDeadwood();
            foreach (var target in _dropTargetHintReferences)
            {
                target.DropTargetHintAdorner = null;
            }
        }

        /// <summary>
        /// Update drop hint for the current element when drag leaves a drop target.
        /// </summary>
        /// <param name="dropHandler">The <see cref="IDropTarget"/> for the operation</param>
        /// <param name="dragInfo">The <see cref="IDragInfo"/> initiating the drag</param>
        /// <param name="sender">The target element of the drag</param>
        public static void OnDragLeave(object sender, IDropTarget dropHandler, IDragInfo dragInfo)
        {
            var wrapper = _dropTargetHintReferences.Find(m => m.Target == sender);
            if (wrapper != null)
            {
                var dropHintInfo = new DropHintInfo(dragInfo);
                dropHandler.DropHint(dropHintInfo);
                UpdateHintAdorner(wrapper, dropHintInfo.DropTargetHintAdorner, new DropHintData(dropHintInfo.DropTargetHintState, dropHintInfo.DropHintText));
            }
        }

        /// <summary>
        /// Update drop hint for the current element.
        /// </summary>
        /// <param name="dropInfo"></param>
        /// <param name="sender"></param>
        public static void DragOver(object sender, IDropInfo dropInfo)
        {
            var wrapper = _dropTargetHintReferences.Find(m => m.Target == sender);
            if (wrapper != null)
            {
                UpdateHintAdorner(wrapper, dropInfo.DropTargetHintAdorner, new DropHintData(dropInfo.DropTargetHintState, dropInfo.DropHintText));
            }
        }

        private static void UpdateHintAdorner(DropTargetHintWeakReference weakReference, [CanBeNull] Type adornerType, DropHintData hintData)
        {
            if (adornerType == null)
            {
                // Discard existing adorner as new parameter is not set
                weakReference.DropTargetHintAdorner = null;
                return;
            }

            if (weakReference.DropTargetHintAdorner != null && weakReference.DropTargetHintAdorner.GetType() != adornerType)
            {
                // Type has changed, so we need to remove the old adorner.
                weakReference.DropTargetHintAdorner = null;
            }

            if (weakReference.DropTargetHintAdorner == null)
            {
                // Create new adorner if it does not exist.
                var dataTemplate = DragDrop.TryGetDropHintDataTemplate(weakReference.Target);
                weakReference.DropTargetHintAdorner = DropTargetHintAdorner.CreateHintAdorner(adornerType, weakReference.Target, dataTemplate, hintData);
            }

            weakReference.DropTargetHintAdorner?.Update(hintData);
        }

        /// <summary>
        /// Helper method for getting available hint drop targets.
        /// </summary>
        /// <returns></returns>
        private static List<DropTargetHintWeakReference> GetVisibleTargets()
        {
            return _dropTargetHintReferences.FindAll(m => m.Target?.IsVisible == true && DragDrop.GetIsDropTarget(m.Target));
        }

        /// <summary>
        /// Clean deadwood in case we are holding on to references to dead objects.
        /// </summary>
        private static void CleanDeadwood()
        {
            _dropTargetHintReferences.RemoveAll((m => !m.IsAlive));
        }

        /// <summary>
        /// Get the default drop hint template if none other has been provided.
        /// </summary>
        /// <returns></returns>
        public static DataTemplate GetDefaultDropHintTemplate()
        {
            var rootBorderName = "RootBorder";
            var backgroundBrush = new SolidColorBrush(SystemColors.HighlightColor) { Opacity = 0.3 };
            backgroundBrush.Freeze();
            var activeBackgroundBrush = new SolidColorBrush(SystemColors.HighlightColor) { Opacity = 0.5 };
            activeBackgroundBrush.Freeze();
            var errorBackgroundBrush = new SolidColorBrush(Colors.DarkRed) { Opacity = 0.3 };
            errorBackgroundBrush.Freeze();

            var template = new DataTemplate();

            var hintStateBinding = new Binding(nameof(DropHintData.HintState));
            var activeSetter = new Setter
                               {
                                   Property = Border.BackgroundProperty,
                                   TargetName = rootBorderName,
                                   Value = activeBackgroundBrush
                               };
            var errorSetter = new Setter
                              {
                                  Property = Border.BackgroundProperty,
                                  TargetName = rootBorderName,
                                  Value = errorBackgroundBrush
                              };

            template.Triggers.Add(new DataTrigger { Binding = hintStateBinding, Value = DropHintState.Active, Setters = { activeSetter } });
            template.Triggers.Add(new DataTrigger { Binding = hintStateBinding, Value = DropHintState.Error, Setters = { errorSetter } });

            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetValue(TextBlock.TextProperty, new Binding(nameof(DropHintData.HintText)));
            textBlockFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            textBlockFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textBlockFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Create a Border factory
            var borderFactory = new FrameworkElementFactory(typeof(Border))
                                {
                                    Name = rootBorderName
                                };
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.CornflowerBlue);
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            borderFactory.SetValue(Border.BackgroundProperty, backgroundBrush);

            // Set the TextBlock as the child of the Border
            borderFactory.AppendChild(textBlockFactory);

            // Set the Border as the root of the visual tree
            template.VisualTree = borderFactory;

            return template;
        }
    }
}