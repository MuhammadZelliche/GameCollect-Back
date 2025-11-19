using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetSecret.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        // --- Clés étrangères ---
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }

        // --- Données de la review ---
        [Range(1, 5)] // La note est entre 1 et 5
        public int Note { get; set; }

        public string? Commentaire { get; set; } // Commentaire (nullable)

        public DateTime DatePublication { get; set; } = DateTime.UtcNow;

        // --- Objets de navigation (pour EF Core) ---
    public User User { get; set; } = null!;
    public Game Game { get; set; } = null!;
    }
}