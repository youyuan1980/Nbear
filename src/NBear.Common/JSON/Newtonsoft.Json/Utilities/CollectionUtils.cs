#region License
// Copyright 2006 James Newton-King
// http://www.newtonsoft.com
//
// This work is licensed under the Creative Commons Attribution 2.5 License
// http://creativecommons.org/licenses/by/2.5/
//
// You are free:
//    * to copy, distribute, display, and perform the work
//    * to make derivative works
//    * to make commercial use of the work
//
// Under the following conditions:
//    * You must attribute the work in the manner specified by the author or licensor:
//          - If you find this component useful a link to http://www.newtonsoft.com would be appreciated.
//    * For any reuse or distribution, you must make clear to others the license terms of this work.
//    * Any of these conditions can be waived if you get permission from the copyright holder.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal static class CollectionUtils
	{
		public static bool IsNullOrEmpty(ICollection collection)
		{
			if (collection != null)
			{
				return (collection.Count == 0);
			}
			return true;
		}

		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			if (collection != null)
			{
				return (collection.Count == 0);
			}
			return true;
		}

		public static bool IsNullOrEmptyOrDefault<T>(IList<T> list)
		{
			if (IsNullOrEmpty<T>(list))
				return true;

			return ReflectionUtils.ElementsUnitializedValue<T>(list);
		}

		public static bool ValuesUnitialized(Array array)
		{
			if (IsNullOrEmpty(array))
				throw new ArgumentException("Array is null or empty", "array");

			Type arrayElementType = array.GetType().GetElementType();

			//arrayType.

			return true;
		}
		
		public static bool ValuesUnitialized<T>(IList<T> list)
		{
			if (IsNullOrEmpty(list))
				throw new ArgumentException("List is null or empty", "list");

			return true;
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end)
		{
			return Slice<T>(list, start, end, null);
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end, int? step)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (step == 0)
				throw new ArgumentException("Step cannot be zero.", "step");

			List<T> slicedList = new List<T>();

			if (list.Count == 0)
				return slicedList;

			int s = step ?? 1;

			int startIndex = ((start < 0) ? list.Count + start : start) ?? 0;
			int endIndex = ((end < 0) ? list.Count + end : end) ?? list.Count;


			startIndex = Math.Max(startIndex, 0);
			endIndex = Math.Min(endIndex, list.Count - 1);

			for (int i = startIndex; i < endIndex; i += s)
			{
				slicedList.Add(list[i]);
			}


			return slicedList;
		}
	}
}