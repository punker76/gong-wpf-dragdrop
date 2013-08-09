using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  public class TypeUtilities
  {
    public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
    {
      var type = GetCommonBaseClass(source);
      var listType = typeof(List<>).MakeGenericType(type);
      var addMethod = listType.GetMethod("Add");
      var list = listType.GetConstructor(Type.EmptyTypes).Invoke(null);

      foreach (var o in source) {
        addMethod.Invoke(list, new[] { o });
      }

      return (IEnumerable)list;
    }

    public static Type GetCommonBaseClass(IEnumerable e)
    {
      var types = e.Cast<object>().Select(o => o.GetType()).ToArray<Type>();
      return GetCommonBaseClass(types);
    }

    public static Type GetCommonBaseClass(Type[] types)
    {
      if (types.Length == 0) {
        return typeof(object);
      }

      var ret = types[0];

      for (var i = 1; i < types.Length; ++i) {
        if (types[i].IsAssignableFrom(ret)) {
          ret = types[i];
        } else {
          // This will always terminate when ret == typeof(object)
          while (!ret.IsAssignableFrom(types[i])) {
            ret = ret.BaseType;
          }
        }
      }

      return ret;
    }
  }
}