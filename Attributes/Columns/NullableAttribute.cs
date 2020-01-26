using System;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class NullableAttribute : ORMColumnAttribute
    {
    }
}
