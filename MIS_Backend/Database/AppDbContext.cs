using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database.Models;

namespace MIS_Backend.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Specialyti> Specialytis { get; set; }
    }
}
