using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqToSQL.Net.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<T>(this PropertyInfo method)
        {
            return method.CustomAttributes.Count(x => x.AttributeType == typeof(T)) > 0;
        }

        public static bool IsCollection(this Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType) return true; // ref-type

            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>

            return false; // value-type
        }

        public static object ConvertTo(this ExpandoObject source, Type destinationType)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException("source");
            if (destinationType == null)
                throw new ArgumentNullException("destination");

            var _propertyMap =
            destinationType
            .GetProperties()
            .ToDictionary(
                p => p.Name.ToLower(),
                p => p
            );

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.

            object obj = Activator.CreateInstance(destinationType);

            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    var propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (!propType.IsByRef && propType.Name != "Nullable`1")
                        {
                            // Throw if type is a value type 
                            // but not Nullable<>
                            throw new ArgumentException("not nullable");
                        }
                    }
                    p.SetValue(obj, Convert.ChangeType(kv.Value, propType), null);
                }
            }
            return obj;
        }
    }
}
