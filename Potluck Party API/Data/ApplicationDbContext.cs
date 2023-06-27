using Microsoft.EntityFrameworkCore;
using Potluck_Party_API.Models;

namespace Potluck_Party_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<RSVP> RSVP { get; set; }
    }
}
