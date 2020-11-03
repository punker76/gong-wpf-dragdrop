using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class TypeUtilities
    {
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var type = GetCommonBaseClass(source);

            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");

            var list = listType.GetConstructor(Type.EmptyTypes)?.Invoke(null);

            foreach (var o in source)
            {
                addMethod.Invoke(list, new[] { o });
            }

            return list as IEnumerable;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                {
                    ret = types[i];
                }
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (ret is { } && !ret.IsAssignableFrom(types[i]))
                    {
                        ret = ret.BaseType;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the enumerable as list.
        /// If enumerable is an ICollectionView then it returns the SourceCollection as list.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>Returns a list.</returns>
        public static IList TryGetList(this IEnumerable enumerable)
        {
            if (enumerable is ICollectionView collectionView)
            {
                return collectionView.SourceCollection as IList;
            }

            if (enumerable is IList list)
            {
                return list;
            }

            return enumerable?.OfType<object>().ToList();
        }

        /// <summary>
        /// Checks if the given collection is a ObservableCollection&lt;&gt;
        /// </summary>
        /// <param name="collection">The collection to test.</param>
        /// <returns>True if the collection is a ObservableCollection&lt;&gt;</returns>
        public static bool IsObservableCollection(this IList collection)
        {
            return collection != null
                   && collection.GetType().IsGenericType
                   && collection.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>);
        }

        /// <summary>
        /// Checks if both collections are the same ObservableCollection&lt;&gt;
        /// </summary>
        /// <param name="collection1">The first collection to test.</param>
        /// <param name="collection2">The second collection to test.</param>
        /// <returns>True if both collections are the same ObservableCollection&lt;&gt;</returns>
        public static bool IsSameObservableCollection(this IList collection1, IList collection2)
        {
            return collection1 != null
                   && ReferenceEquals(collection1, collection2)
                   && collection1.IsObservableCollection();
        }
    }
}