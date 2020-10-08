using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Stats entity for queries.
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// Transaction uniqe id.
        /// </summary>
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Query start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Query end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Query type.
        /// </summary>
        public QueryType QueryType { get; set; }
    }
}
