using Microsoft.EntityFrameworkCore;
using MySuparApp.Models.Authentication;

namespace MySuparApp.Shared // Replace with your actual namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSet properties (tables)
        public DbSet<UserModel> Users { get; set; }

        public DbSet<UserCredModel> UserCred { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().ToTable("UserDetails").HasKey(u => u.UserId); ;
            modelBuilder.Entity<UserCredModel>().ToTable("UserCreds");
        }
    }
}