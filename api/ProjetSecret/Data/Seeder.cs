using Bogus;
using Microsoft.EntityFrameworkCore;
using ProjetSecret.Models;

namespace ProjetSecret.Data
{
    public class Seeder
    {
        private readonly GameCollectDbContext _context;

        public Seeder(GameCollectDbContext context)
        {
            _context = context;
        }

        // Petite extension utilitaire pour convertir proprement en UTC
        private static DateTime ToUtc(DateTime date)
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Local).ToUniversalTime();
        }

        public async Task SeedAsync()
        {
            // Étape 1 : Vérifier si la BDD est déjà remplie
            if (await _context.Users.AnyAsync() || await _context.Games.AnyAsync())
            {
                Console.WriteLine("La base de données contient déjà des données. Seeding annulé.");
                return;
            }

            Console.WriteLine("Début du seeding...");

            // --- 1. Créer un Admin ---
            var admin = new User
            {
                Username = "admin",
                Email = "admin@gamecollect.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "admin",
                DateCreation = DateTime.UtcNow
            };
            _context.Users.Add(admin);

            // --- 2. Créer 100 Faux Utilisateurs ---
            var userFaker = new Faker<User>("fr")
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => BCrypt.Net.BCrypt.HashPassword("Password123!"))
                .RuleFor(u => u.Role, f => "user")
                .RuleFor(u => u.DateCreation, f => ToUtc(f.Date.Past(3)));

            var fakeUsers = userFaker.Generate(100);
            await _context.Users.AddRangeAsync(fakeUsers);

            // --- 3. Créer 500 Faux Jeux ---
            var gameFaker = new Faker<Game>("fr")
                .RuleFor(g => g.Titre, f => f.Commerce.ProductName())
                .RuleFor(g => g.Plateforme, f => f.PickRandom("PlayStation 5", "Xbox Series X", "Nintendo Switch", "PC", "Mega Drive"))
                .RuleFor(g => g.AnneeSortie, f => f.Random.Int(1980, 2024))
                .RuleFor(g => g.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(g => g.Rarete, f => f.PickRandom("Commun", "Rare", "Très Rare"));

            var fakeGames = gameFaker.Generate(500);
            await _context.Games.AddRangeAsync(fakeGames);

            // Sauvegarde pour récupérer les IDs
            await _context.SaveChangesAsync();
            Console.WriteLine("Utilisateurs et Jeux créés.");

            // --- 4. Créer 1000 UserGame ---
            var userIds = await _context.Users.Select(u => u.UserId).ToListAsync();
            var gameIds = await _context.Games.Select(g => g.GameId).ToListAsync();
            var random = new Random();

            var userGames = new List<UserGame>();

            for (int i = 0; i < 1000; i++)
            {
                userGames.Add(new UserGame
                {
                    UserId = userIds[random.Next(userIds.Count)],
                    GameId = gameIds[random.Next(gameIds.Count)],
                    DateAjout = DateTime.UtcNow.AddDays(-random.Next(30)),
                    NotePerso = random.Next(1, 6)
                });
            }

            var uniqueUserGames = userGames
                .GroupBy(ug => new { ug.UserId, ug.GameId })
                .Select(g => g.First());

            await _context.UserGames.AddRangeAsync(uniqueUserGames);

            // --- 5. Créer 500 Avis ---
            var reviewFaker = new Faker<Review>("fr")
                .RuleFor(r => r.UserId, f => f.PickRandom(userIds))
                .RuleFor(r => r.GameId, f => f.PickRandom(gameIds))
                .RuleFor(r => r.Note, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Commentaire, f => f.Lorem.Paragraph(1))
                .RuleFor(r => r.DatePublication, f => ToUtc(f.Date.Past(1)));

            var fakeReviews = reviewFaker.Generate(500);

            var uniqueReviews = fakeReviews
                .GroupBy(r => new { r.UserId, r.GameId })
                .Select(g => g.First());

            await _context.Reviews.AddRangeAsync(uniqueReviews);

            await _context.SaveChangesAsync();
            Console.WriteLine("Collections et Avis créés. Seeding terminé !");
        }
    }
}
