
using System.ComponentModel.DataAnnotations; // Pour les annotations comme [Key]
using System.ComponentModel.DataAnnotations.Schema; 

namespace ProjetSecret.Models
{
    [Table("Users")] 
    public class User
    {
        [Key] 
        public int UserId { get; set; }

        [Required] 
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow; 

        [MaxLength(50)]
        public string Role { get; set; } = "user"; 

        // --- Relations ---
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}