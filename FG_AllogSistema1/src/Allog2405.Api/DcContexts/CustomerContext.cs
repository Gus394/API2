using Allog2405.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Allog2405.Api.DbContexts;

public class CustomerContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;

    public CustomerContext(DbContextOptions<CustomerContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasData(
                new Customer()
                {
                    id = 1,
                    firstName = "Linus Torvalds",
                    lastName = "asdasd",
                    cpf = "73473943096",


                },
                new Customer
                {
                    id = 2,
                    firstName = "Linus Torvalds",
                    lastName = "asdasd",
                    cpf = "73473943096",
                });

        modelBuilder.Entity<Address>()
            .HasData(
                    new Address()
                    {
                        Id = 1,
                        Street = "Verão do Cometa",
                        City = "Elvira",
                        CustomerId = 1
                    },
                    new Address()
                    {
                        Id = 2,
                        Street = "Borboletas Psicodélicas",
                        City = "Perobia",
                        CustomerId = 1
                    },
                    new Address()
                    {
                        Id = 3,
                        Street = "Canção Excêntrica",
                        City = "Salandra",
                        CustomerId = 2
                    }
            );


        base.OnModelCreating(modelBuilder);
    }

}