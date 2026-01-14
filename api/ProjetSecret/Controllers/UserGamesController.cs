using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetSecret.Data;
using ProjetSecret.DTOs;
using ProjetSecret.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjetSecret.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserGamesController : ControllerBase
    {
        private readonly GameCollectDbContext _context;

        public UserGamesController(GameCollectDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // GET: api/usergames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUserGames()
        {
            var userId = GetCurrentUserId();
            
            var userGames = await _context.UserGames
                .Include(ug => ug.Game)
                .Include(ug => ug.User)
                .Where(ug => ug.UserId == userId)
                .Select(ug => new
                {
                    userGameId = ug.UserGameId,
                    userId = ug.UserId,
                    username = ug.User.Username,
                    gameId = ug.GameId,
                    title = ug.Game.Titre,
                    imageUrl = ug.Game.ImageUrl,
                    platform = ug.Game.Plateforme,
                    dateAjout = ug.DateAjout,
                    notePerso = ug.NotePerso
                })
                .ToListAsync();

            return Ok(userGames);
        }

        // POST: api/usergames
        [HttpPost]
        public async Task<ActionResult<UserGameDto>> AddToCollection([FromBody] AddGameToCollectionDto dto)
        {
            var userId = GetCurrentUserId();

            // Vérifier si le jeu existe
            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null)
            {
                return NotFound(new { message = "Jeu non trouvé" });
            }

            // Vérifier si le jeu est déjà dans la collection
            var existingUserGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == dto.GameId);
            
            if (existingUserGame != null)
            {
                return Conflict(new { message = "Ce jeu est déjà dans votre collection" });
            }

            var userGame = new UserGame
            {
                UserId = userId,
                GameId = dto.GameId,
                DateAjout = DateTime.UtcNow
            };

            _context.UserGames.Add(userGame);
            await _context.SaveChangesAsync();

            // Charger les relations pour le DTO
            await _context.Entry(userGame).Reference(ug => ug.Game).LoadAsync();
            await _context.Entry(userGame).Reference(ug => ug.User).LoadAsync();

            var userGameDto = new UserGameDto
            {
                UserId = userGame.UserId,
                Username = userGame.User.Username,
                GameId = userGame.GameId,
                GameTitre = userGame.Game.Titre,
                NotePerso = userGame.NotePerso,
                DateAjout = userGame.DateAjout
            };

            return CreatedAtAction(nameof(GetUserGames), new { id = userGame.UserGameId }, userGameDto);
        }

        // DELETE: api/usergames/5
        [HttpDelete("{gameId}")]
        public async Task<IActionResult> RemoveFromCollection(int gameId)
        {
            var userId = GetCurrentUserId();

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);

            if (userGame == null)
            {
                return NotFound(new { message = "Ce jeu n'est pas dans votre collection" });
            }

            _context.UserGames.Remove(userGame);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}