// --- Fichier : Controllers/UsersController.cs (Complet) ---

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetSecret.Data;
using ProjetSecret.Models;
using ProjetSecret.DTOs;
using BCrypt.Net; // Pour le hachage
using Microsoft.AspNetCore.Authorization; // <-- LE USING MANQUANT !
using Microsoft.IdentityModel.Tokens; // Pour le JWT
using System.IdentityModel.Tokens.Jwt; // Pour le JWT
using System.Security.Claims; // Pour le JWT
using System.Text; // Pour le JWT

namespace ProjetSecret.Controllers
{
    [ApiController]
    [Authorize] // Protégé par défaut
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GameCollectDbContext _context;
        private readonly IConfiguration _configuration;

        // Injection du DbContext ET de la Configuration (pour le JWT_SECRET)
        public UsersController(GameCollectDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            // Seul un "admin" devrait pouvoir faire ça
            // On vérifie le rôle présent dans le token
            if (!User.IsInRole("admin"))
            {
                return Forbid("Accès réservé aux administrateurs.");
            }

            return await _context.Users
                .Select(user => new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    DateCreation = user.DateCreation,
                    Role = user.Role
                })
                .ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            // Conversion en DTO sûr
            return Ok(new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DateCreation = user.DateCreation,
                Role = user.Role
            });
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto updateDto)
        {
            // 1. Récupérer l'ID de l'utilisateur depuis le token
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            // 2. Vérifier que l'utilisateur modifie SON PROPRE profil
            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("Vous ne pouvez pas modifier le profil d'un autre utilisateur.");
            }

            // 3. Trouver l'utilisateur en BDD
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // 4. Vérifier l'unicité du nouvel email/username
            if (user.Email != updateDto.Email && await _context.Users.AnyAsync(u => u.Email == updateDto.Email))
            {
                return BadRequest("Cet email n'est pas acceptée.");
            }
            if (user.Username != updateDto.Username && await _context.Users.AnyAsync(u => u.Username == updateDto.Username))
            {
                return BadRequest("Ce nom d'utilisateur est déjà pris.");
            }

            // 5. Appliquer les changements et sauvegarder
            user.Username = updateDto.Username;
            user.Email = updateDto.Email;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - standard pour un PUT réussi
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // 1. Récupérer l'ID et le Rôle de l'utilisateur depuis le token
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            // 2. Vérifier les droits : on est admin OU on supprime son propre compte
            if (id.ToString() != currentUserId && currentUserRole != "Admin")
            {
                return StatusCode(403, new { message = "Vous n'avez pas les droits pour supprimer ce compte." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}