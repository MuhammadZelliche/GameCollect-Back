using System.ComponentModel.DataAnnotations;

namespace ProjetSecret.DTOs
{
    // Ce qu'un utilisateur est autorisé à modifier sur son profil
    public class UserUpdateDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}