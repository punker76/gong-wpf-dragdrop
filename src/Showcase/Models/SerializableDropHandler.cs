using System;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace Showcase.WPF.DragDrop.Models
{
    public class SerializableDropHandler : IDropTarget
    {
#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragEnter(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public void DragOver(IDropInfo dropInfo)
        {
            var wrapper = GetSerializableWrapper(dropInfo);
            if (wrapper != null && dropInfo.TargetCollection != null)
            {
                dropInfo.Effects = ShouldCopyData(dropInfo, wrapper.DragDropCopyKeyState) ? DragDropEffects.Copy : DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc />
        public void DragLeave(IDropInfo dropInfo)
        {
            // nothing here
        }
#endif

        /// <inheritdoc />
        public void Drop(IDropInfo dropInfo)
        {
            var wrapper = GetSerializableWrapper(dropInfo);
            if (wrapper != null && dropInfo.TargetCollection != null)
            {
                // at this point the drag info can be null, cause the other app doesn't know it
                var insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;
                var destinationList = dropInfo.TargetCollection.TryGetList();
                var copyData = ShouldCopyData(dropInfo, wrapper.DragDropCopyKeyState);

                if (!copyData)
                {
                    var sourceList = dropInfo.DragInfo?.SourceCollection?.TryGetList();
                    if (sourceList != null)
                    {
                        foreach (var o in wrapper.Items)
                        {
                            var index = sourceList.IndexOf(o);
                            if (index != -1)
                            {
                                sourceList.RemoveAt(index);
                                // so, is the source list the destination list too ?
                                if (destinationList != null && Equals(sourceList, destinationList) && index < insertIndex)
                                {
                                    --insertIndex;
                                }
                            }
                        }
                    }
                }

                if (destinationList != null)
                {
                    // check for cloning
                    var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy)
                                    || dropInfo.Effects.HasFlag(DragDropEffects.Link);
                    foreach (var o in wrapper.Items)
                    {
                        var obj2Insert = o;
                        if (cloneData)
                        {
                            var cloneable = o as ICloneable;
                            if (cloneable != null)
                            {
                                obj2Insert = cloneable.Clone();
                            }
                        }

                        destinationList.Insert(insertIndex++, obj2Insert);
                    }
                }
            }
        }

        private static SerializableWrapper GetSerializableWrapper(IDropInfo dropInfo)
        {
            var data = dropInfo.Data;

            var dataObject = data as DataObject;
            if (dataObject != null)
            {
                var dataFormat = DataFormats.GetDataFormat(DataFormats.Serializable);
                data = dataObject.GetDataPresent(dataFormat.Name) ? dataObject.GetData(dataFormat.Name) : data;
            }

            var wrapper = data as SerializableWrapper;
            return wrapper;
        }

        private static bool ShouldCopyData(IDropInfo dropInfo, DragDropKeyStates dragDropCopyKeyState)
        {
            // default should always the move action/effect
            if (dropInfo == null)
            {
                return false;
            }

            var copyData = ((dragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dragDropCopyKeyState))
                           || dragDropCopyKeyState.HasFlag(DragDropKeyStates.LeftMouseButton);
            return copyData;
        }
    }
}