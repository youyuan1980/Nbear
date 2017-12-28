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
using System.Reflection;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal static class ReflectionUtils
	{
		public static bool IsInstantiatableType(Type t)
		{
			if (t == null)
				throw new ArgumentNullException("t");

			if (t.IsAbstract || t.IsInterface || t.IsArray)
				return false;

			if (t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null) == null)
				return false;

			return true;
		}

		public static bool IsNullableType(Type underlyingType)
		{
			if (underlyingType.IsValueType)
			{
                if (!underlyingType.IsGenericType && typeof(Nullable<>).IsAssignableFrom(underlyingType))
					return false;
			}
			return true;
		}

		public static bool IsValueTypeUnitializedValue(ValueType value)
		{
			return IsValueTypeUnitializedValue(value, value.GetType());
		}

		private static bool IsValueTypeUnitializedValue(ValueType value, Type valueType)
		{
			if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return (value == null);
			else
				return value.Equals((ValueType) Activator.CreateInstance(valueType));
		}

		public static object GetTypeUnitializedValue(Type type)
		{
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			else if (type.IsClass)
				return null;
			else
				throw new ArgumentException("Type is neither a ValueType or a Class", "type");
		}

		public static bool IsObjectUnitializedValue(object value, Type valueType)
		{
			if (valueType.IsValueType)
				return IsValueTypeUnitializedValue((ValueType) value, valueType);
			else if (valueType.IsClass)
				return (value == null);
			else
				throw new ArgumentException("Type is neither a ValueType or a Class", "valueType");
		}

		public static bool IsSubClass(Type type, Type check)
		{
			if (type == null || check == null)
				return false;

			if (type == check)
				return true;

			if (check.IsInterface)
			{
				foreach (Type t in type.GetInterfaces())
				{
					if (IsSubClass(t, check)) return true;
				}
			}
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				if (IsSubClass(type.GetGenericTypeDefinition(), check))
					return true;
			}
			return IsSubClass(type.BaseType, check);
		}
		
		public static Type GetListType(Type listType)
		{
			if (listType.IsArray)
				return listType.GetElementType();
			else if (listType.IsGenericType && IsSubClass(listType.GetGenericTypeDefinition(), typeof(List<>)))
				return listType.GetGenericArguments()[0];
			else
				return null;
		}

		public static Type GetDictionaryValueType(object values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			Type dictionaryType = values.GetType();

			if (dictionaryType.IsGenericType && IsSubClass(dictionaryType.GetGenericTypeDefinition(), typeof(Dictionary<,>)))
				return dictionaryType.GetGenericArguments()[1];
			else if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
				return null;
			else
				throw new Exception("Bad type");
		}

		public static bool ElementsUnitializedValue<T>(IList<T> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			Type elementType = GetListType(values.GetType());

			if (elementType.IsValueType)
			{
				object unitializedValue = GetTypeUnitializedValue(elementType);

				for (int i = 0; i < values.Count; i++)
				{
					if (!values[i].Equals(unitializedValue))
						return false;
				}
			}
			else if (elementType.IsClass)
			{
				for (int i = 0; i < values.Count; i++)
				{
					if (values[i] != null)
						return false;
				}
			}
			else
			{
				throw new ArgumentException("Type is neither a ValueType or a Class", "valueType");
			}

			return true;
		}

		public static Type GetMemberUnderlyingType(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo) member).FieldType;
				case MemberTypes.Property:
					return ((PropertyInfo) member).PropertyType;
				case MemberTypes.Event:
					return ((EventInfo) member).EventHandlerType;
				default:
					throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
			}
		}

		public static bool IsIndexedProperty(MemberInfo member)
		{
			if (member == null)
				throw new ArgumentNullException("member");

			PropertyInfo propertyInfo = member as PropertyInfo;

			if (propertyInfo != null)
				return IsIndexedProperty(propertyInfo);
			else
				return false;
		}

		public static bool IsIndexedProperty(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			return (property.GetIndexParameters().Length > 0);
		}

		public static object GetMemberValue(MemberInfo member, object target)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo) member).GetValue(target);
				case MemberTypes.Property:
					try
					{
						return ((PropertyInfo) member).GetValue(target, null);
					}
					catch (TargetParameterCountException e)
					{
						throw new ArgumentException("MemberInfo has index parameters", "member", e);
					}
				default:
					throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
			}
		}

		public static void SetMemberValue(MemberInfo member, object target, object value)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					((FieldInfo) member).SetValue(target, value);
					break;
				case MemberTypes.Property:
					((PropertyInfo) member).SetValue(target, value, null);
					break;
				default:
					throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
			}
		}

		public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
		{
			return GetFieldsAndProperties(typeof(T), bindingAttr);
		}

		public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
		{
			List<MemberInfo> targetMembers = new List<MemberInfo>();

			targetMembers.AddRange(type.GetFields(bindingAttr));
			targetMembers.AddRange(type.GetProperties(bindingAttr));

			return targetMembers;
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider) where T : Attribute
		{
			return GetAttribute<T>(attributeProvider, false, true);
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider, bool errorOnMultiple) where T : Attribute
		{
			return GetAttribute<T>(attributeProvider, errorOnMultiple, true);
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider, bool errorOnMultiple, bool inherit) where T : Attribute
		{
			T[] attributes = GetAttributes<T>(attributeProvider, inherit);

			if (CollectionUtils.IsNullOrEmpty<T>(attributes))
				return null;
			else if (attributes.Length == 1)
				return attributes[0];
			else
				throw new AmbiguousMatchException(string.Format("Multiple attributes of type '{0}' found.", typeof(T).Name));
		}

		public static T[] GetAttributes<T>(ICustomAttributeProvider attributeProvider, bool inherit) where T : Attribute
		{
			return (T[]) attributeProvider.GetCustomAttributes(typeof(T), inherit);
		}
	}
}
