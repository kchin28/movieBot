using dbot.Models;
using Microsoft.EntityFrameworkCore;

namespace dbot.Data
{
    public class MovieBotContext : DbContext
    {
        public MovieBotContext(DbContextOptions<MovieBotContext> options) : base(options)
        {
        }
        public DbSet<User> Users {get;set;}
        public DbSet<Nomination> WeeklyNominations { get; set; }
        public DbSet<Vote> WeeklyVotes {get;set;}
        public DbSet<Session> Sessions {get;set;}
        //public DbSet<Movie> Movies {get;set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nomination>().ToTable("WeeklyNominations");
            modelBuilder.Entity<Vote>().ToTable("WeeklyVotes");
            modelBuilder.Entity<Session>().ToTable("Sessions");
            modelBuilder.Entity<User>().ToTable("Users");
        }

    }
}