using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class ForeignKeyAttribute : ORMColumnAttribute
    {
        public ForeignKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public string KeyName { get; set; }
    }
}
