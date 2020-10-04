using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("insertMultipleCustomer")]
        public async Task<IActionResult> InsertMultipleCustomerAsync()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 4; i++)
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

        [HttpGet("viewFunction")]
        public async Task<IActionResult> GetDataFromViewFunctionAsync()
        {
            dbContext.Database.ExecuteSqlRaw(
                            @"CREATE VIEW vw_KeylessCustomerEntity AS 
                            SELECT Name FROM Customers");

            var data = dbContext.KeylessCustomerEntities.ToList();
            return NoContent();
        }
    }
}
