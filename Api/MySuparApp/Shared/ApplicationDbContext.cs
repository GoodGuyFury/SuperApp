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
        public DbSet<EntityUserModel> Users { get; set; }

        public DbSet<EntityUserCredModel> UserCred { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityUserModel>()
                        .HasIndex(u => u.UserId)
                        .IsUnique(); // UserId is the primary key

            modelBuilder.Entity<EntityUserModel>()
                        .HasIndex(u => u.Email)
                        .IsUnique(); // Ensure Email is unique

            modelBuilder.Entity<EntityUserModel>()
                      .Property(u => u.Role)
                      .HasConversion(
                          v => v.ToString(), // Convert enum to string for storage
                          v => (UserRole)Enum.Parse(typeof(UserRole), v)) // Convert string back to enum
                      .HasMaxLength(10); // Set max length for Role

            modelBuilder.Entity<EntityUserModel>()
                        .ToTable("UserDetails")
                        .HasKey(u => u.UserInt);

            modelBuilder.Entity<EntityUserCredModel>()
                        .ToTable("UserCreds")
                        .HasKey(u => u.UserInt);

            modelBuilder.Entity<EntityUserCredModel>()
                         .HasOne(uc => uc.User) // Each UserCred has one User
                         .WithMany(u => u.UserCreds) // Each User can have many UserCreds
                         .HasForeignKey(uc => uc.UserInt) // Foreign key property
                         .OnDelete(DeleteBehavior.Cascade);

        }
    }
}