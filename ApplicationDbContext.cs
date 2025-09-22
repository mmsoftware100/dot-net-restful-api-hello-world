using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Blood> BloodGroup { get; set; }
        public DbSet<WishBottle> WishBottles { get; set; }
        public DbSet<CatchRecord> CatchRecords { get; set; }
    }
}
