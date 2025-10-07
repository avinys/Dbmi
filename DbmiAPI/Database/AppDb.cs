using BdmiAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Database
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Users
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();
            b.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // MovieGenre M:N
            b.Entity<MovieGenre>().HasKey(mg => new { mg.MovieId, mg.GenreId });

            // Movies
            b.Entity<Movie>()
                .HasOne(m => m.UploadedByUser)
                .WithMany(u => u.UploadedMovies)
                .HasForeignKey(m => m.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Movie>().HasIndex(m => m.Title);

            // Reviews
            b.Entity<Review>()
                .HasOne(r => r.Movie).WithMany(m => m.Reviews).HasForeignKey(r => r.MovieId);

            b.Entity<Review>()
                .HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId);

            // One review per user per movie
            b.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();

            // Cascade delete reviews if movie is deleted
            b.Entity<Review>()
                 .HasOne(r => r.Movie)
                 .WithMany(m => m.Reviews)
                 .HasForeignKey(r => r.MovieId)
                 .OnDelete(DeleteBehavior.Cascade);

            b.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "deleted",
                Email = "deleted@system.local",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
