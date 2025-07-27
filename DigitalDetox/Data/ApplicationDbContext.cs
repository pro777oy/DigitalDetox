using DigitalDetox.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalDetox.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentResponse> StudentResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasIndex(s => new { s.Class, s.Roll })
                .IsUnique(); 
        }
    }
}