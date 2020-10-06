using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Interceptors
{
    /// <summary>
    /// Interception for db command.
    /// </summary>
    public class SampleInterception : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            command.CommandText += "order by 1 desc";
            return base.ReaderExecuting(command, eventData, result);
        }
    }
}
