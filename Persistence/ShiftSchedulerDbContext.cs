using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Models;

namespace ShiftScheduler.Persistence
{
    public class ShiftSchedulerDbContext : DbContext
    {
        public ShiftSchedulerDbContext(DbContextOptions<ShiftSchedulerDbContext> options)
        : base(options)
        {
        }
        public DbSet<Shift> Shifts { get; set; }

        public DbSet<Engineer> Engineers { get; set; }
    }
}