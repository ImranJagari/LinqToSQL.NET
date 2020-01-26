using LinqToSQL.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IgnoreAttribute : ORMColumnAttribute
    {
    }
}
