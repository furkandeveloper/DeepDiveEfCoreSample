using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Base Entity.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Entity create date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Entity Update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Entity status flag.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
