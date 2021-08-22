using dbot.Models;
using Microsoft.EntityFrameworkCore;

namespace dbot.Data
{
    public class MovieBotContext : DbContext
    {
        public MovieBotContext(DbContextOptions<MovieBotContext> options) : base(options)
        {
        }

        public DbSet<Nomination> WeeklyNominations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nomination>().ToTable("Nomination");
          //  modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
         //   modelBuilder.Entity<Student>().ToTable("Student");
        }

    }
}