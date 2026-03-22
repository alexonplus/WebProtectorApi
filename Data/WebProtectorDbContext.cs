using Microsoft.EntityFrameworkCore;
using WebProtectorApi.Entities;

namespace WebProtectorApi.Data
{
    public class WebProtectorDbContext : DbContext
    {
        public WebProtectorDbContext(DbContextOptions<WebProtectorDbContext> options)
            : base(options)
        {
        }

        public DbSet<ScanReport> ScanReports { get; set; } = null!;

        // adding Users DbSet for authentication
        public DbSet<User> Users { get; set; } = null!;
    }
}