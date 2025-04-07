using Microsoft.EntityFrameworkCore;

namespace HealthChecks.API.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
