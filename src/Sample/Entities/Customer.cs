using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Customer entity.
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// pk
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Customer surname
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Customer phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// relation one to many
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
    }
}
