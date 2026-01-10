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
        
        [Range(1, 5)] 
        public int Note { get; set; }

        public string? Commentaire { get; set; }

        public DateTime DatePublication { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}