using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetSecret.Models
{
    [Table("UserGames")]
    public class UserGame
    {
        [Key]
        public int UserGameId { get; set; }

        // --- Clés étrangères ---
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }

        // --- Données de la relation ---
        public DateTime DateAjout { get; set; } = DateTime.UtcNow;

        public int? NotePerso { get; set; } // Note personnelle (nullable)

        // --- Objets de navigation (pour EF Core) ---
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}