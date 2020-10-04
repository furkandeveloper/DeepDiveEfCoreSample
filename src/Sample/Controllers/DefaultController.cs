using Microsoft.AspNetCore.Mvc;
using Sample.Context;
using Sample.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly SampleDbContext dbContext;

        public DefaultController(SampleDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetIncludeQueryAsync()
        {
            return Ok();
        }

        [HttpGet("")]
        public async Task<IActionResult> InsertMultipleCustomerAsync()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 3; i++)
            {
                var customer = new Customer()
                {
                    Name = i + ". customerName",
                    Surname = i + ". customerSurname",
                    PhoneNumber = "xxxxxxxxxxx",
                };
                customers.Add(customer);
            }
            await dbContext.Customers.AddRangeAsync(customers);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
