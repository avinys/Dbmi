using BdmiAPI.Database;
using BdmiAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BdmiAPI.Infrastructure // <- pick your namespace
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync(AppDb db, CancellationToken ct = default)
        {
            // Ensure the special system user exists with Id = 1
            var deleted = await db.Users.FirstOrDefaultAsync(u => u.Id == 1, ct);
            if (deleted is null)
            {
                // Make sure your real table name is "Users". If you renamed it via ToTable("..."),
                // adjust the raw SQL accordingly.
                await db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO `Users` (`Id`,`Username`,`Email`,`IsSystem`,`CreatedAt`) VALUES (1,'deleted','deleted@system.local',1,UTC_TIMESTAMP())",
                    ct
                );
            }

            var candidates = new[]
            {
                new User { Username = "alice",   Email = "alice@example.com"   },
                new User { Username = "bob",     Email = "bob@example.com"     },
                new User { Username = "charlie", Email = "charlie@example.com" },
                new User { Username = "diana",   Email = "diana@example.com"   }
            };

            foreach (var u in candidates)
            {
                var exists = await db.Users.AnyAsync(x => x.Username == u.Username || x.Email == u.Email, ct);
                if (!exists) db.Users.Add(u);
            }

            await db.SaveChangesAsync(ct);
        }

        public static async Task SeedGenresAsync(AppDb db, CancellationToken ct = default)
        {
            var genres = new[]
            {
                new Genre { Name="Action",      Description="High energy, fights, chases, and explosions." },
                new Genre { Name="Comedy",      Description="Humorous and entertaining." },
                new Genre { Name="Drama",       Description="Emotional themes and character development." },
                new Genre { Name="Sci-Fi",      Description="Futuristic or advanced technology." },
                new Genre { Name="Fantasy",     Description="Magic and imaginary worlds." },
                new Genre { Name="Horror",      Description="Frightening, suspenseful, supernatural." },
                new Genre { Name="Romance",     Description="Love stories." },
                new Genre { Name="Thriller",    Description="Suspenseful plots and twists." },
                new Genre { Name="Documentary", Description="Non-fictional storytelling." },
                new Genre { Name="Animation",   Description="Animated works." }
            };

            foreach (var g in genres)
            {
                // Check by Name
                var exists = await db.Genres.AnyAsync(x => x.Name == g.Name, ct);
                if (!exists) db.Genres.Add(g);
            }

            await db.SaveChangesAsync(ct);
        }

        public static async Task SeedMoviesAsync(AppDb db, CancellationToken ct = default)
        {
            var movies = new (string Title, string Desc, int Year, int Duration, int UploaderId, int[] Genres)[]
            {
                ("Inception", "A thief enters dreams to steal secrets, but aims to plant an idea.", 2010, 148, 2, new[]{4,8}), // Sci-Fi + Thriller
                ("Interstellar", "Explorers travel through a wormhole to save humanity.", 2014, 169, 2, new[]{4,3}),          // Sci-Fi + Drama
                ("The Dark Knight", "Batman faces Joker in Gotham.", 2008, 152, 3, new[]{1,8}),                               // Action + Thriller
                ("Titanic", "A romance unfolds on the doomed ship.", 1997, 195, 4, new[]{3,7}),                               // Drama + Romance
                ("Spirited Away", "A girl enters a magical spirit world.", 2001, 125, 5, new[]{5,10}),                        // Fantasy + Animation
                ("Get Out", "A visit to a girlfriend’s family turns sinister.", 2017, 104, 3, new[]{6,8})                     // Horror + Thriller
            };

            foreach (var m in movies)
            {
                var exists = await db.Movies.AnyAsync(x => x.Title == m.Title && x.ReleaseYear == m.Year, ct);
                if (!exists)
                {
                    var entity = new Movie
                    {
                        Title = m.Title,
                        Description = m.Desc,
                        ReleaseYear = m.Year,
                        DurationMin = m.Duration,
                        UploadedByUserId = m.UploaderId,
                        MovieGenres = m.Genres.Select(gid => new MovieGenre { GenreId = gid }).ToList()
                    };
                    db.Movies.Add(entity);
                }
            }

            await db.SaveChangesAsync(ct);
        }

        public static async Task SeedReviewsAsync(AppDb db, CancellationToken ct = default)
        {
            var reviews = new[]
            {
                new Review { MovieId = 1, UserId = 3, Score = 9, Title = "Mind-bending!", Body = "Nolan at his best." },
                new Review { MovieId = 1, UserId = 4, Score = 8, Title = "Great but complex", Body = "Had to rewatch to understand fully." },
                new Review { MovieId = 2, UserId = 5, Score = 10, Title = "Epic", Body = "Visually stunning and emotional." },
                new Review { MovieId = 3, UserId = 2, Score = 9, Title = "Iconic", Body = "Ledger’s Joker is unforgettable." },
                new Review { MovieId = 4, UserId = 5, Score = 8, Title = "Classic", Body = "Heartbreaking and beautiful." },
                new Review { MovieId = 5, UserId = 2, Score = 9, Title = "Magical", Body = "One of the best animations ever." },
                new Review { MovieId = 6, UserId = 4, Score = 7, Title = "Creepy", Body = "Loved the social commentary." }
            };

            foreach (var r in reviews)
            {
                var exists = await db.Reviews.AnyAsync(x => x.MovieId == r.MovieId && x.UserId == r.UserId, ct);
                if (!exists) db.Reviews.Add(r);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
