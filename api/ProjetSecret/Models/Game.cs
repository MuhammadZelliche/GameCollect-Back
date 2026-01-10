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

        public string? ImageUrl { get; set; }

        [MaxLength(50)]
        public string? Rarete { get; set; }

        // --- Relations ---
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}