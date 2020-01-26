using System;
using System.Globalization;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class DefaultAttribute : ORMColumnAttribute
    {
        public DefaultAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; set; }

        public override string Apply()
        {
            string value = DefaultValue.ToString();
            if (DefaultValue.GetType() == typeof(float))
                value = ((float)DefaultValue).ToString(CultureInfo.InvariantCulture);
            else if (DefaultValue.GetType() == typeof(decimal))
                value = ((decimal)DefaultValue).ToString(CultureInfo.InvariantCulture);
            else if (DefaultValue.GetType() == typeof(double))
                value = ((double)DefaultValue).ToString(CultureInfo.InvariantCulture);

            return $"DEFAULT {value}";
        }
    }
}
