using Microsoft.EntityFrameworkCore;
using Potluck_Party_API.Models;
using System.Reflection.Metadata;

namespace Potluck_Party_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }

        public DbSet<Dish> Dish { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
