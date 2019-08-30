using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace Showcase.WPF.DragDrop.Models
{
    [Serializable]
    public class SerializableWrapper
    {
        public IEnumerable<object> Items { get; set; }

        public DragDropKeyStates DragDropCopyKeyState { get; set; }
    }

    public class SerializableDragHandler : IDragSource
    {
        private bool alreadyDropped = false;

        /// <inheritdoc />
        public void StartDrag(IDragInfo dragInfo)
        {
            alreadyDropped = false;
            var items = dragInfo.SourceItems.OfType<object>().ToList();
            var wrapper = new SerializableWrapper()
                          {
                              Items = items,
                              DragDropCopyKeyState = DragDropKeyStates.ControlKey //dragInfo.DragDropCopyKeyState
                          };
            dragInfo.Data = wrapper;
            dragInfo.DataFormat = DataFormats.GetDataFormat(DataFormats.Serializable);
            dragInfo.Effects = dragInfo.Data != null ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.None;
        }

        /// <inheritdoc />
        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        /// <inheritdoc />
        public void Dropped(IDropInfo dropInfo)
        {
            alreadyDropped = true;
        }

        /// <inheritdoc />
        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            if (alreadyDropped || dragInfo == null)
            {
                return;
            }

            // the drag operation has finished on another app
            if (operationResult != DragDropEffects.None)
            {
                if (operationResult.HasFlag(DragDropEffects.Move))
                {
                    var sourceList = dragInfo.SourceCollection.TryGetList();
                    var items = dragInfo.SourceItems.OfType<object>().ToList();
                    if (sourceList != null)
                    {
                        foreach (var o in items)
                        {
                            sourceList.Remove(o);
                        }
                    }
                    alreadyDropped = true;
                }
            }
        }

        /// <inheritdoc />
        public void DragCancelled()
        {
        }

        /// <inheritdoc />
        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }
    }
}