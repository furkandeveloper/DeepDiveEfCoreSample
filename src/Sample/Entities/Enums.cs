using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Query type for stats.
    /// </summary>
    public enum QueryType
    {
        Select = 1,
        Insert,
        Update,
        Delete
    }
}
