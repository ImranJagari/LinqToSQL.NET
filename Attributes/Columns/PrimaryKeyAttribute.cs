using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(string columnName)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        }

        public string ColumnName { get; set; }
    }
}
