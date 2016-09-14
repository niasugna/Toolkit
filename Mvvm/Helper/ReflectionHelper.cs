using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public static class ReflectionHelper
    {
        public static void MapAllFields(object source, object dst)
        {
            System.Reflection.FieldInfo[] ps = source.GetType().GetFields();
            foreach (var item in ps)
            {
                var o = item.GetValue(source);
                var p = dst.GetType().GetField(item.Name);
                if (p != null)
                {
                    Type t = Nullable.GetUnderlyingType(p.FieldType) ?? p.FieldType;
                    object safeValue = (o == null) ? null : Convert.ChangeType(o, t);
                    p.SetValue(dst, safeValue);
                }
            }
        }
        public static object GetPropertyValue(object srcobj, string propertyName)
        {
            if (srcobj == null)
                return null;

            object obj = srcobj;

            // Split property name to parts (propertyName could be hierarchical, like obj.subobj.subobj.property
            string[] propertyNameParts = propertyName.Split('.');

            foreach (string propertyNamePart in propertyNameParts)
            {
                if (obj == null) return null;

                // propertyNamePart could contain reference to specific 
                // element (by index) inside a collection
                if (!propertyNamePart.Contains("["))
                {
                    PropertyInfo pi = obj.GetType().GetProperty(propertyNamePart);
                    if (pi == null) return null;
                    obj = pi.GetValue(obj, null);
                }
                else
                {   // propertyNamePart is areference to specific element 
                    // (by index) inside a collection
                    // like AggregatedCollection[123]
                    //   get collection name and element index
                    int indexStart = propertyNamePart.IndexOf("[") + 1;
                    string collectionPropertyName = propertyNamePart.Substring(0, indexStart - 1);

                    //   get collection object
                    PropertyInfo pi = obj.GetType().GetProperty(collectionPropertyName);
                    if (pi == null) return null;
                    object unknownCollection = pi.GetValue(obj, null);
                    //   try to process the collection as array
                    if (unknownCollection.GetType().IsArray)
                    {
                        int collectionElementIndex = Int32.Parse(propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1));
                        object[] collectionAsArray = unknownCollection as Array[];
                        obj = collectionAsArray[collectionElementIndex];
                    }
                    else if (unknownCollection is System.Collections.IList)
                    {
                        int collectionElementIndex = Int32.Parse(propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1));

                        System.Collections.IList collectionAsList = unknownCollection as System.Collections.IList;
                        if (collectionAsList != null)
                        {
                            obj = collectionAsList[collectionElementIndex];
                        }

                    }
                    else if (unknownCollection is System.Collections.IDictionary)
                    {
                        System.Collections.IDictionary dictionary = unknownCollection as System.Collections.IDictionary;
                        if (dictionary != null)
                        {

                            string key = propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1).Trim('\"');
                            bool success = Regex.IsMatch(key, "\"[\\s]\"");
                            ICollection<string> values = dictionary.Values as ICollection<string>;

                            if (dictionary.Contains(key))
                                obj = dictionary[key];
                            else
                                obj = null;
                        }

                    }
                    else
                    {
                        // ??? Unsupported collection type
                    }
                }
            }
            return obj;
        }
        public static Object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
        public static T GetPropValue<T>(this Object obj, String name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return default(T);
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
