using Microsoft.EntityFrameworkCore;
using GlobalATM.Models;

namespace GlobalATM.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}
        public DbSet<User> Users {get;set;}

        public DbSet<Account> Accounts {get;set;}

        public DbSet<Checking> Checkings {get;set;}
        public DbSet<Saving> Savings {get;set;}
        public DbSet<Transaction> Transactions {get;set;}
    }
}