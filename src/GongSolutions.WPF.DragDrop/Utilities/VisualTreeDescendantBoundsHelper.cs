using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    internal static class VisualTreeDescendantBoundsHelper
    {
        private static Assembly PresentationCoreAssembly { get; } = typeof(ContentElement).Assembly;

        private static Assembly WindowsBaseAssembly { get; } = typeof(System.ComponentModel.GroupDescription).Assembly;

        private static FieldInfo OffsetFieldInfo { get; } = typeof(Visual).GetField("_offset", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Type UncommonFieldType { get; } = WindowsBaseAssembly.GetType("System.Windows.UncommonField`1");

        private static Func<Type, MethodInfo> GetValueMethodInfo { get; } = t => UncommonFieldType.MakeGenericType(t).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public);

        private static Type BitmapEffectStateType { get; } = PresentationCoreAssembly.GetType("System.Windows.Media.Effects.BitmapEffectState");

        private static Type VisualFlagsType { get; } = PresentationCoreAssembly.GetType("System.Windows.Media.VisualFlags");

        // UncommonFields
        private static FieldInfo ScrollableAreaClipFieldFieldInfo { get; } = typeof(Visual).GetField("ScrollableAreaClipField", BindingFlags.Static | BindingFlags.NonPublic);

        private static FieldInfo EffectFieldFieldInfo { get; } = typeof(Visual).GetField("EffectField", BindingFlags.Static | BindingFlags.NonPublic);

        private static FieldInfo TransformFieldFieldInfo { get; } = typeof(Visual).GetField("TransformField", BindingFlags.Static | BindingFlags.NonPublic);

        private static FieldInfo BitmapEffectStateFieldFieldInfo { get; } = typeof(Visual).GetField("BitmapEffectStateField", BindingFlags.Static | BindingFlags.NonPublic);

        private static FieldInfo ClipFieldFieldInfo { get; } = typeof(Visual).GetField("ClipField", BindingFlags.Static | BindingFlags.NonPublic);

        private static MethodInfo IsEmptyRenderBoundsMethodInfo { get; } = typeof(Visual).GetMethod("IsEmptyRenderBounds", BindingFlags.Instance | BindingFlags.NonPublic);

        private static MethodInfo CheckFlagsAndMethodInfo { get; } = typeof(Visual).GetMethod("CheckFlagsAnd", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { VisualFlagsType }, null);

        private static PropertyInfo EffectMappingPropertyInfo { get; } = typeof(Effect).GetProperty("EffectMapping", BindingFlags.Instance | BindingFlags.NonPublic);

        private static MethodInfo UnitToWorldMethodInfo { get; } = typeof(Effect).GetMethod("UnitToWorld", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Rect), typeof(Rect) }, null);

        private static PropertyInfo IsIdentityPropertyInfo { get; } = typeof(Transform).GetProperty("IsIdentity", BindingFlags.Instance | BindingFlags.NonPublic);

        // Microsoft utility methods
        private static MethodInfo TransformRectMethodInfo { get; } = WindowsBaseAssembly.GetType("MS.Internal.MatrixUtil").GetMethod("TransformRect", BindingFlags.Static | BindingFlags.NonPublic);

        private static MethodInfo RectHasNaNMethodInfo { get; } = WindowsBaseAssembly.GetType("MS.Internal.DoubleUtil").GetMethod("RectHasNaN", BindingFlags.Static | BindingFlags.Public);

        public static Rect GetVisibleDescendantBounds(Visual visual)
        {
            Rect boundingBoxSubGraph = CalculateSubgraphBoundsInnerSpace(visual, false);

            if ((bool)RectHasNaNMethodInfo.Invoke(null, new object[] { boundingBoxSubGraph }))
            {
                boundingBoxSubGraph.X = Double.NegativeInfinity;
                boundingBoxSubGraph.Y = Double.NegativeInfinity;
                boundingBoxSubGraph.Width = Double.PositiveInfinity;
                boundingBoxSubGraph.Height = Double.PositiveInfinity;
            }

            return boundingBoxSubGraph;
        }

        private static Rect CalculateSubgraphBoundsInnerSpace(Visual visual, bool renderBounds)
        {
            Rect boundingBoxSubGraph = Rect.Empty;

            int count = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < count; i++)
            {
                Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;

                if (child != null && ((bool)CheckFlagsAndMethodInfo.Invoke(child, new object[] { Enum.Parse(VisualFlagsType, "VisibilityCache_Visible") }) || (bool)CheckFlagsAndMethodInfo.Invoke(child, new object[] { Enum.Parse(VisualFlagsType, "VisibilityCache_TakesSpace") })))
                {
                    Rect boundingBoxSubGraphChild = CalculateSubgraphBoundsOuterSpace(child, renderBounds);
                    boundingBoxSubGraph.Union(boundingBoxSubGraphChild);
                }
            }

            Rect contentBounds = VisualTreeHelper.GetContentBounds(visual);

            if (renderBounds && (bool)IsEmptyRenderBoundsMethodInfo.Invoke(visual, new object[] { contentBounds }))
            {
                contentBounds = Rect.Empty;
            }

            boundingBoxSubGraph.Union(contentBounds);

            return boundingBoxSubGraph;
        }

        private static Rect CalculateSubgraphBoundsOuterSpace(Visual visual, bool renderBounds)
        {
            Rect boundingBoxSubGraph = CalculateSubgraphBoundsInnerSpace(visual, renderBounds);

            if ((bool)CheckFlagsAndMethodInfo.Invoke(visual, new object[] { Enum.Parse(VisualFlagsType, "NodeHasEffect") }))
            {
                Effect effect = (Effect)GetValueMethodInfo(typeof(Effect)).Invoke(EffectFieldFieldInfo.GetValue(visual), new object[] { visual });

                if (effect != null)
                {
                    Rect unitBounds = new Rect(0, 0, 1, 1);
                    Rect unitTransformedBounds = (EffectMappingPropertyInfo.GetValue(effect) as GeneralTransform).TransformBounds(unitBounds);
                    Rect effectBounds = (Rect)UnitToWorldMethodInfo.Invoke(null, new object[] { unitTransformedBounds, boundingBoxSubGraph });

                    boundingBoxSubGraph.Union(effectBounds);
                }
                else
                {
                    Debug.Assert(GetValueMethodInfo(BitmapEffectStateType).Invoke(BitmapEffectStateFieldFieldInfo.GetValue(visual), new object[] { visual }) != null);
                }
            }

            Geometry clip = (Geometry)GetValueMethodInfo(typeof(Geometry)).Invoke(ClipFieldFieldInfo.GetValue(visual), new object[] { visual });

            if (clip != null)
            {
                boundingBoxSubGraph.Intersect(clip.Bounds);
            }

            Transform transform = (Transform)GetValueMethodInfo(typeof(Transform)).Invoke(TransformFieldFieldInfo.GetValue(visual), new object[] { visual });

            if ((transform != null) && (!(bool)IsIdentityPropertyInfo.GetValue(transform)))
            {
                Matrix matrix = transform.Value;
                TransformRectMethodInfo.Invoke(null, new object[] { boundingBoxSubGraph, matrix });
            }

            if (!boundingBoxSubGraph.IsEmpty)
            {
                var _offset = (Vector)OffsetFieldInfo.GetValue(visual);
                boundingBoxSubGraph.X += _offset.X;
                boundingBoxSubGraph.Y += _offset.Y;
            }

            Rect? scrollClip = (Rect?)GetValueMethodInfo(typeof(Rect?)).Invoke(ScrollableAreaClipFieldFieldInfo.GetValue(visual), new object[] { visual });

            if (scrollClip.HasValue)
            {
                boundingBoxSubGraph.Intersect(scrollClip.Value);
            }

            if ((bool)RectHasNaNMethodInfo.Invoke(null, new object[] { boundingBoxSubGraph }))
            {
                boundingBoxSubGraph.X = Double.NegativeInfinity;
                boundingBoxSubGraph.Y = Double.NegativeInfinity;
                boundingBoxSubGraph.Width = Double.PositiveInfinity;
                boundingBoxSubGraph.Height = Double.PositiveInfinity;
            }

            return boundingBoxSubGraph;
        }
    }
}