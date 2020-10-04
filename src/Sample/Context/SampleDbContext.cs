using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Entities;
using Sample.Generations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Context
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions options) : base(options)
        {
        }

        protected SampleDbContext()
        {
        }

        #region Tables
        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseLoggerFactory(loggerFactory));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity
                    .HasKey(pk => pk.CustomerId);

                entity
                    .Property(p => p.CustomerId)
                    .HasValueGenerator<GuidGenerator>()
                    .ValueGeneratedOnAdd();

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity
                    .Property(p => p.Surname)
                    .HasMaxLength(600)
                    .IsRequired();

                entity
                    .Property(p => p.PhoneNumber)
                    .HasMaxLength(11)
                    .IsRequired();

                entity
                    .Property(p => p.IsActive)
                    .HasValueGenerator<TrueGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.CreateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.UpdateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAdd().
                    IsRequired();

                entity
                    .HasMany(m => m.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(fk => fk.CustomerId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity
                    .HasKey(pk => pk.OrderId);

                entity
                    .Property(p => p.OrderCode)
                    .HasValueGenerator<OrderCodeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.Total)
                    .IsRequired();

                entity
                    .Property(p => p.IsShipped)
                    .HasValueGenerator<FalseGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.IsActive)
                    .HasValueGenerator<TrueGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.CreateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity
                    .Property(p => p.UpdateDate)
                    .HasValueGenerator<DateTimeGenerator>()
                    .ValueGeneratedOnAddOrUpdate().
                    IsRequired();

                entity
                    .HasOne(o => o.Customer)
                    .WithMany(m => m.Orders)
                    .HasForeignKey(fk => fk.CustomerId);
            });
        }

        public static readonly ILoggerFactory loggerFactory
         = LoggerFactory.Create(builder =>
         {
             builder
                 .AddFilter((category, level) =>
                     category == DbLoggerCategory.Database.Command.Name
                     && level == LogLevel.Information)
                 .AddDebug();
         });
    }
}
