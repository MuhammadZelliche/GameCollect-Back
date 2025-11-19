using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetSecret.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; } = null!;

        [MaxLength(100)]
        public string Plateforme { get; set; } = null!;

        public int AnneeSortie { get; set; }

        public string? ImageUrl { get; set; } // '?' indique que c'est nullable

        [MaxLength(50)]
        public string? Rarete { get; set; }

        // --- Relations ---
        // Un jeu peut être dans plusieurs collections (UserGame)
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

        // Un jeu peut recevoir plusieurs Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}