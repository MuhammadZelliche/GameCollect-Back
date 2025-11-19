
using System.ComponentModel.DataAnnotations; // Pour les annotations comme [Key]
using System.ComponentModel.DataAnnotations.Schema; // Pour [Table]

namespace ProjetSecret.Models
{
    [Table("Users")] // Nom de la table en BDD
    public class User
    {
        [Key] // Indique que c'est la clé primaire
        public int UserId { get; set; }

        [Required] // Indique non-nul
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress] // Validation d'email
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow; // Valeur par défaut

        [MaxLength(50)]
        public string Role { get; set; } = "user"; // Valeur par défaut

        // --- Relations ---
        // Un utilisateur peut avoir plusieurs UserGame (sa collection)
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

        // Un utilisateur peut écrire plusieurs Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}