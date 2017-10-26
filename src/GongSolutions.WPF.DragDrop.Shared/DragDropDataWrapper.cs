using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GongSolutions.WPF.DragDrop.Shared
{
  [Serializable]
  public class DragDropDataWrapper : ISerializable
  {
    public DragDropDataWrapper(object innerData)
    {
      InnerData = innerData;
    }

    public DragDropDataWrapper(SerializationInfo info, StreamingContext context)
    {
      if (info == null) throw new ArgumentNullException("info");

      InnerData = info.GetValue("InnerData", typeof(object));
    }

    public object InnerData { get; set; }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null) throw new ArgumentNullException("info");

      info.AddValue("InnerData", InnerData);
    }
  }
}
