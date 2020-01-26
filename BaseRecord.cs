using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSQL.Net
{
    public class BaseRecord
    {
        public DateTime? LastTimeModified { get; set; }
    }
    public class BaseRecord<KeyType> : BaseRecord
    {
        public KeyType Id { get; set; }
    }
}
