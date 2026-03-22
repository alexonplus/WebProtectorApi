using Microsoft.EntityFrameworkCore;
using WebProtectorApi.Models;

namespace WebProtectorApi.Data
{
    // This class handles the connection between your code and the database
    public class WebProtectorDbContext : DbContext
    {
        public WebProtectorDbContext(DbContextOptions<WebProtectorDbContext> options)
            : base(options)
        {
        }

        // This creates the 'ScanReports' table in your database
        public DbSet<ScanReport> ScanReports { get; set; } = null!;
    }
}