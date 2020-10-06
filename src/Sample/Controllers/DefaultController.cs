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
        public async Task<IActionResult> GetDataAsync()
        {
            var data = dbContext.Customers.ToList();
            var dataIgnoreGlobalFilter = dbContext.Customers.IgnoreQueryFilters().ToList();
            return Ok();
        }

        [HttpGet("insertMultipleCustomer")]
        public async Task<IActionResult> InsertMultipleCustomerAsync()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
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

            //List<Order> orders = new List<Order>();
            //for (int i = 0; i < 100; i++)
            //{
            //    orders.Add(new Order()
            //    {
            //        CustomerId = customers[i].CustomerId,
            //        Total = i*85,
            //        UpdateDate = DateTime.UtcNow
            //    });
            //}

            //await dbContext.Orders.AddRangeAsync(orders);
            //await dbContext.SaveChangesAsync();
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

        [HttpGet("getQuery")]
        public async Task<IActionResult> GetQueryAsync()
        {
            var query = dbContext.Customers.Where(c => c.Name == "customer");
            return Ok(query.ToQueryString());
        }

        [HttpGet("splitQuery")]
        public async Task<IActionResult> GetSplitQueryAsync()
        {
            var query = dbContext
                            .Customers
                            .AsSplitQuery()
                            .Include(x=>x.Orders.OrderBy(a=>a.OrderId).Take(5))
                            .ToList();
            return NoContent();
        }
    }
}
