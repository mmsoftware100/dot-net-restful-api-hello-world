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
    }
}
