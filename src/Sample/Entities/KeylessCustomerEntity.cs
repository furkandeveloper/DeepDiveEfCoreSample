using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Entities
{
    /// <summary>
    /// Keyless Entity Type
    /// </summary>
    [Keyless]
    public class KeylessCustomerEntity
    {
        public string Name { get; set; }
    }
}
