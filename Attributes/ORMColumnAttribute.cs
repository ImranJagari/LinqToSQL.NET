using System;

namespace LinqToSQL.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ORMColumnAttribute : Attribute, IColumnAttribute
    {
        public virtual string Apply()
        {
            return "";
        }
    }
}
