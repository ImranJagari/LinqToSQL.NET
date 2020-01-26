using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}
