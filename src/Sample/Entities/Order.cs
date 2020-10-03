using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Order entity.
    /// </summary>
    public class Order : BaseEntity
    {
        /// <summary>
        /// pk
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Order code || order number
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// Total price
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Is order shipped?
        /// </summary>
        public bool IsShipped { get; set; }

        /// <summary>
        /// relation one to many
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}
