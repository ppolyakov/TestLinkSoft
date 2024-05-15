using Microsoft.EntityFrameworkCore;
using SocialNetworkAnalyzerApi.Entities;

namespace SocialNetworkAnalyzerApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<ImportData> Imports { get; set; }
    }
}