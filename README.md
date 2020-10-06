![1 jITJLx9hL96w4LEEXB7GFQ](https://user-images.githubusercontent.com/47147484/94990812-78c78e80-0587-11eb-95a0-24540320e13e.png)

# DeepDiveEfCoreSample
This project includes deep dive entity framework core samples.


One more sprint is going on in your lifetime, heeey developer, what have you learned in this sprint where you made a dozen more mistakes that you can tell in your career and be careful not to do it again? Won't they ask you how many of these methods you have refactored? Make sure they ask. 😃
In daily life, most of us run sprints, while planning the sprint, the team; he learns where to run and buys the tools he needs to take in his team bag, and the whistle rang out and the race started ...
This series of techniques that I try to maintain while running such intense sprints is a therapy for me. 😃 I hope you guys have the same feelings as me.

In this article of the technical conversation series, I will examine EF Core, which we mainly use in Mobiroller, and try to make a deep dive.
ORM (Object Relational Mapping)
It is a programming technique that connects the objects in the application with the database technology we use and acts as a bridge between these two ends.
ORM is a programming technique, not a tool. Some ORM tools;
* Entity Framework
* Entity Framework Core
* Dapper
* nHibernate

These tools, which we will prefer regarding the operation of the project, can sometimes vary. While some projects use EF Core, we can use Dapper or a hybrid model in some projects. They all have advantages over each other, but since the purpose of this article is to examine the EF Core tool in depth, I end my sentence here. 😃

```
Packages;
Install-Package Microsoft.EntityFrameworkCore -Version 5.0.0-rc.1.20451.13
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.0-rc.1.20451.13
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 5.0.0-rc.1.20451.13
Install-Package Microsoft.Extensions.Logging -Version 5.0.0-rc.1.20451.14
```

I will perform reviews on Customer and Order entities.
I have collected the parameters I use common on all entities on BaseEntity.

```csharp
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
```

```csharp
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
```

```csharp
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
        /// fk
        /// </summary>
        public Guid CustomerId { get; set; }

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
```

Now that the preparations are complete, let the journey begin. 😃 🛫

### Value Generator
In the process of adding or updating, we want to change the values ​​of some fields with a fixed value or depending on a specific algorithm.
For example, let's apply CreateDate, UpdateDate, IsActive fields with the most primitive method in Insert and Update operations.

```
// Update
var customer = dbContext.Customers.First();
customer.CreateDate = DateTime.UtcNow;
customer.UpdateDate = DateTime.UtcNow;
customer.IsActive = false;
```

Yes, this method definitely works, but writing such a logic for each entity will negatively affect the readability and cleanliness of the code.
In addition to the scenario I mentioned above, let's add a new module. We have a field named OrderCode on the Order entity. When a new order is placed, let's provide a new OrderCode that starts with MBR.

```
var order = new Order(){OrderCode = "MBR"+Guid.NewGuid().ToString()}
```
In addition, different scenarios can be added, in this case, you can use value generator structures to control the increasing scenarios from one place and simplify your application.

```csharp
    /// <summary>
    /// Date time generator. Using create and update data parameter.
    /// </summary>
    public class DateTimeGenerator : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => false;

        protected override object NextValue(EntityEntry entry)
        {
            return DateTime.UtcNow;
        }
    }
```

```csharp
    /// <summary>
    /// Order code value generator.
    /// </summary>
    public class OrderCodeGenerator : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => false;

        protected override object NextValue(EntityEntry entry)
        {
            return "MBR" + Guid.NewGuid().ToString();
        }
    }
```

It will be sufficient to mark these fields in DbContext `OnModelCreating`.

```csharp
modelBuilder.Entity<Customer>(entity =>
            {
                entity
                    .Property(p => p.CreateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();
            });

modelBuilder.Entity<Order>(entity =>
            {
                entity
                    .Property(p => p.OrderCode)
                    .HasValueGenerator<OrderCodeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.CreateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();
            });
```

### When to Insert? When Bulk Insert?
A feature I learned while preparing this article, I was very surprised to read it and immediately shared this information with my teammates. Why do we need a difference between Insert and Bulk Insert parameters?
To summarize this topic, I will talk about the daily problems we face at Mobiroller.
Since we perform microservice transformation in mobirols, we build the structures that we cut off from the monolith application in microservice architecture. Usually the first question we ask is "Which database should we use for this domain?" is happening. As we make frequent database changes, data migration has become one of our routines. For this reason, Insert and Bulk Insert difference is very important for Mobiroller.
Long story short 😃 If the number of records you add is between 0 and 3, Entity Framework Core generates as many Insert commands as the number of records, and produces a single Insert command when the number of records is more.

```csharp
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
```

Generated SQL Query:

```
Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (73ms) [Parameters=[@p0='?' (DbType = Guid), @p1='?' (DbType = DateTime2), @p2='?' (DbType = Boolean), @p3='?' (Size = 200), @p4='?' (Size = 11), @p5='?' (Size = 600), @p6='?' (DbType = DateTime2), @p7='?' (DbType = Guid), @p8='?' (DbType = DateTime2), @p9='?' (DbType = Boolean), @p10='?' (Size = 200), @p11='?' (Size = 11), @p12='?' (Size = 600), @p13='?' (DbType = DateTime2), @p14='?' (DbType = Guid), @p15='?' (DbType = DateTime2), @p16='?' (DbType = Boolean), @p17='?' (Size = 200), @p18='?' (Size = 11), @p19='?' (Size = 600), @p20='?' (DbType = DateTime2), @p21='?' (DbType = Guid), @p22='?' (DbType = DateTime2), @p23='?' (DbType = Boolean), @p24='?' (Size = 200), @p25='?' (Size = 11), @p26='?' (Size = 600), @p27='?' (DbType = DateTime2)], CommandType='Text', CommandTimeout='30']
SET NOCOUNT ON;
INSERT INTO [Customers] ([CustomerId], [CreateDate], [IsActive], [Name], [PhoneNumber], [Surname], [UpdateDate])
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6),
(@p7, @p8, @p9, @p10, @p11, @p12, @p13),
(@p14, @p15, @p16, @p17, @p18, @p19, @p20),
(@p21, @p22, @p23, @p24, @p25, @p26, @p27);
```

Migration
Especially if you have a multi-tenancy application at the db level, migration db-model incompatibilities can cause you headaches. 🤒
To avoid this problem, you can add a little code in Startup.cs. 😃

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env,SampleDbContext dbContext)
{
    // Apply pending migrations but context final version is important.
    dbContext.Database.Migrate();
    
    // Delete database.
    dbContext.Database.EnsureDeleted();
    
    // Apply migrations.
    dbContext.Database.EnsureCreated();
}
```

### Keyless Entity

```csharp
DefaultController.cs
        [HttpGet("viewFunction")]
        public async Task<IActionResult> GetDataFromViewFunctionAsync()
        {
            dbContext.Database.ExecuteSqlRaw(
                            @"CREATE VIEW vw_KeylessCustomerEntity AS 
                            SELECT Name FROM Customers");

            var data = dbContext.KeylessCustomerEntities.ToList();
            return NoContent();
        }
KeylessCustomerEntity.cs
    /// <summary>
    /// Keyless Entity Type
    /// </summary>
    [Keyless]
    public class KeylessCustomerEntity
    {
        public string Name { get; set; }
    }
SampleDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<KeylessCustomerEntity>(entity =>
    {
        entity.ToView("vw_KeylessCustomerEntity");
    }); 
}
```
