using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using System.Diagnostics;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    public static class TypeUtilities
    {
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var sourceObjects = source.Cast<object>().ToArray();
            var type = GetCommonBaseClass(sourceObjects.Select(o => o.GetType()).Distinct().ToArray());

            try
            {
                var listType = typeof(List<>).MakeGenericType(type);
                if (listType.GetConstructor(Type.EmptyTypes)?.Invoke(null) is IList list)
                {
                    foreach (var o in sourceObjects)
                    {
                        list.Add(o);
                    }

                    return list;
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError($"Could not create a typed list from the source enumerable! {exception}");
            }

            return sourceObjects;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).Distinct().ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }

            var classType = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(classType))
                {
                    classType = types[i];
                }
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (classType is not null && !classType.IsAssignableFrom(types[i]))
                    {
                        classType = classType.BaseType;
                    }
                }
            }

            return classType;
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
        public static bool IsObservableCollection([CanBeNull] this IList collection)
        {
            return collection != null && IsObservableCollectionType(collection.GetType());
        }

        private static bool IsObservableCollectionType([CanBeNull] Type type)
        {
            if (type is null || !typeof(IList).IsAssignableFrom(type))
            {
                return false;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            {
                return true;
            }

            return IsObservableCollectionType(type.BaseType);
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