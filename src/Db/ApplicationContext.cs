using Microsoft.EntityFrameworkCore;

using SoftwareRequirements.Models.Db;

namespace SoftwareRequirements.Db
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options) {}

        public DbSet<Requirement> Requirements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Requirement>().HasMany(r => r.Requirements);
            modelBuilder.Entity<Requirement>().Property(r => r.Write).HasDefaultValue(RequirementWrite.CREATED);
        }
    } 
}