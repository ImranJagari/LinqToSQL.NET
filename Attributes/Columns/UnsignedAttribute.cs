using System;

namespace LinqToSQL.Net.Attributes.Columns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class UnsignedAttribute : ORMColumnAttribute
    {
        public override string Apply()
        {
            return "UNSIGNED";
        }
    }
}
