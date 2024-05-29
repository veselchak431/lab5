namespace MessagingApp1.Data
{
    using Microsoft.EntityFrameworkCore;
    using MessagingApp1.Models;
    using MessagingApp1.Models;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Message>().ToTable("Messages");
        }
    }
}
