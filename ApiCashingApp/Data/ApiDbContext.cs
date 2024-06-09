using ApiCashingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCashingApp.Data
{
    public class ApiDbContext : DbContext
    {

        public DbSet<Driver> Drivers { get; set; }
        public ApiDbContext(DbContextOptions<ApiDbContext> options):base(options)
        {
            
        }
    }
}
