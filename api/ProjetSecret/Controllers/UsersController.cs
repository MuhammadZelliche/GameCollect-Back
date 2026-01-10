using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetSecret.Data;
using ProjetSecret.DTOs;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 

namespace ProjetSecret.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GameCollectDbContext _context;
        private readonly IConfiguration _configuration;

        
        public UsersController(GameCollectDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {

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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            
            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("Vous ne pouvez pas modifier le profil d'un autre utilisateur.");
            }
            
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }
            
            if (user.Email != updateDto.Email && await _context.Users.AnyAsync(u => u.Email == updateDto.Email))
            {
                return BadRequest("Cet email n'est pas acceptée.");
            }
            if (user.Username != updateDto.Username && await _context.Users.AnyAsync(u => u.Username == updateDto.Username))
            {
                return BadRequest("Ce nom d'utilisateur est déjà pris.");
            }
            
            user.Username = updateDto.Username;
            user.Email = updateDto.Email;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
 
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            
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