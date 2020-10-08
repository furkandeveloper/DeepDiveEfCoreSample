using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Context
{
    public class EasyProfilerDbContext : DbContext
    {
        public EasyProfilerDbContext(DbContextOptions options) : base(options)
        {
        }

        protected EasyProfilerDbContext()
        {
        }
    }
}
