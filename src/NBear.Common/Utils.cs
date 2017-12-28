using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace NBear.Common
{
    /// <summary>
    /// The Util class.
    /// </summary>
    public sealed class Util
    {
        private Util()
        {
        }

        /// <summary>
        /// Gets the default value of a specified Type.
        /// </summary>
        /// <returns>The default value.</returns>
        public static object DefaultValue<MemberType>()
        {
            return default(MemberType);
        }

        /// <summary>
        /// Gets the default value of a specified Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object DefaultValue(Type type)
        {
            return typeof(Util).GetMethod("DefaultValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null).MakeGenericMethod(type).Invoke(null, null);
        }

        /// <summary>
        /// Deeply gets property infos.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>Property infos of all the types and there base classes/interfaces</returns>
        public static PropertyInfo[] DeepGetProperties(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return new PropertyInfo[0];
            }
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (Type t in types)
            {
                if (t != null)
                {
                    foreach (PropertyInfo pi in t.GetProperties())
                    {
                        list.Add(pi);
                    }

                    if (t.IsInterface)
                    {
                        Type[] interfaceTypes = t.GetInterfaces();

                        if (interfaceTypes != null)
                        {
                            foreach (PropertyInfo pi in DeepGetProperties(interfaceTypes))
                            {
                                bool isContained = false;

                                foreach (PropertyInfo item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                    else
                    {
                        Type baseType = t.BaseType;

                        if (baseType != typeof(object) && baseType != typeof(ValueType))
                        {
                            foreach (PropertyInfo pi in DeepGetProperties(baseType))
                            {
                                bool isContained = false;

                                foreach (PropertyInfo item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the type of the original type of array.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <returns></returns>
        public static Type GetOriginalTypeOfArrayType(Type returnType)
        {
            return GetType(returnType.ToString().TrimEnd('[', ']'));
        }

        /// <summary>
        /// Gets a type in all loaded assemblies of current app domain.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public static Type GetType(string fullName)
        {
            Type t = null;

            if (fullName.StartsWith("System.Nullable`1["))
            {
                string genericTypeStr = fullName.Substring("System.Nullable`1[".Length).Trim('[', ']');
                if (genericTypeStr.Contains(","))
                {
                    genericTypeStr = genericTypeStr.Substring(0, genericTypeStr.IndexOf(",")).Trim();
                }
                t = typeof(Nullable<>).MakeGenericType(Util.GetType(genericTypeStr));
            }

            if (t != null)
            {
                return t;
            }

            try
            {
                t = Type.GetType(fullName);
            }
            catch
            {
            }

            if (t == null)
            {
                try
                {
                    Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

                    for (int i = asses.Length - 1; i >= 0; i--)
                    {
                        Assembly ass = asses[i];
                        try
                        {
                            t = ass.GetType(fullName);
                        }
                        catch
                        {
                        }

                        if (t != null)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                }
            }

            return t;
        }

        /// <summary>
        /// Deeply get property info from specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static PropertyInfo DeepGetProperty(Type type, string propertyName)
        {
            foreach (PropertyInfo pi in DeepGetProperties(type))
            {
                if (pi.Name == propertyName)
                {
                    return pi;
                }
            }

            return null;
        }

        /// <summary>
        /// Deeps the get field from specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="isPublic">if is public.</param>
        /// <returns>The field info</returns>
        public static FieldInfo DeepGetField(Type type, string name, bool isPublic)
        {
            Type t = type;
            if (t != null)
            {
                FieldInfo fi = (isPublic ? t.GetField(name) : t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic));
                if (fi != null)
                {
                    return fi;
                }

                if (t.IsInterface)
                {
                    Type[] interfaceTypes = t.GetInterfaces();

                    if (interfaceTypes != null)
                    {
                        foreach (Type interfaceType in interfaceTypes)
                        {
                            fi = DeepGetField(interfaceType, name, isPublic);
                            if (fi != null)
                            {
                                return fi;
                            }
                        }
                    }
                }
                else
                {
                    Type baseType = t.BaseType;

                    if (baseType != typeof(object) && baseType != typeof(ValueType))
                    {
                        return DeepGetField(baseType, name, isPublic);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Parses the relative path to absolute path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static string ParseRelativePath(string basePath, string relativePath)
        {
            Check.Require(basePath != null, "basePath could not be null.");
            Check.Require(relativePath != null, "relativePath could not be null.");

            if (relativePath.StartsWith("\\") || relativePath.StartsWith(".\\") || relativePath.Contains(":"))
            {
                return System.IO.Path.GetFullPath(relativePath);
            }

            basePath = basePath.Trim().Replace("/", "\\");
            relativePath = relativePath.Trim().Replace("/", "\\");

            string[] splittedBasePath = basePath.Split('\\');
            string[] splittedRelativePath = relativePath.Split('\\');

            StringBuilder sb = new StringBuilder();
            int parentTokenCount = 0;
            for (int i = 0; i < splittedRelativePath.Length; i++)
            {
                if (splittedRelativePath[i] == "..")
                {
                    parentTokenCount++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < splittedBasePath.Length - parentTokenCount; i++)
            {
                if (!string.IsNullOrEmpty(splittedBasePath[i]))
                {
                    sb.Append(splittedBasePath[i]);
                    sb.Append("\\");
                }
            }

            for (int i = parentTokenCount; i < splittedRelativePath.Length; i++)
            {
                if (!string.IsNullOrEmpty(splittedRelativePath[i]))
                {
                    sb.Append(splittedRelativePath[i]);
                    sb.Append("\\");
                }
            }

            return sb.ToString().TrimEnd('\\');
        }

        /// <summary>
        /// Formats the param val.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string FormatParamVal(object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "null";
            }

            Type type = val.GetType();

            if (type == typeof(string))
            {
                return string.Format("N'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type== typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(1949, 10, 1);
                return string.Format("(CAST('{0}' AS datetime) - CAST('{1}' AS datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (type == typeof(PropertyItem))
            {
                return ((PropertyItem)val).ColumnName ;
            }
            else if (type == typeof(PropertyItemParam))
            {
                return ((PropertyItemParam)val).CustomValue ;
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                return val.ToString();
            }
            else
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
        }
    }
}
