using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using ProjetSecret.Data; 
using ProjetSecret.Models; 
using Microsoft.AspNetCore.Authorization;
using ProjetSecret.DTOs;

namespace ProjetSecret.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")] // -> /api/games
    public class GamesController : ControllerBase
    {
        private readonly GameCollectDbContext _context;
        
        public GamesController(GameCollectDbContext context)
        {
            _context = context;
        }
        
        // POST: api/games
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Game>> CreateGame(Game game)
        {
            _context.Games.Add(game);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetGameById), new { id = game.GameId }, game);
        }
        
        // GET: api/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
        {
            return await _context.Games.Include(r => r.Reviews).Include(u => u.UserGames).Select(g => new GameDto
            {
                Id = g.GameId,
                Titre = g.Titre,
                Plateforme = g.Plateforme,
                AnneeSortie = g.AnneeSortie,
                ImageUrl = g.ImageUrl,
                Rarete = g.Rarete,
                Reviews = g.Reviews.Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    Note = r.Note,
                    Commentaire = r.Commentaire,
                    UserId = r.UserId
                }).ToList(),

                UserGames = g.UserGames.Select(ug => new UserGameDto
                {
                    UserId = ug.UserId,
                    Username = ug.User.Username,
                    GameId = ug.GameId,
                    GameTitre = ug.Game.Titre,
                    NotePerso = ug.NotePerso,
                    DateAjout = ug.DateAjout,
                }).ToList()
            }).ToListAsync();
        }
        
        // GET: api/games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(int id)
        {
            var game = await _context.Games.Include(r => r.Reviews).Include(u => u.UserGames).Where(g => g.GameId == id)
                .Select(g => new GameDto
                {
                    Id = g.GameId,
                    Titre = g.Titre,
                    Plateforme = g.Plateforme,
                    AnneeSortie = g.AnneeSortie,
                    ImageUrl = g.ImageUrl,
                    Rarete = g.Rarete,
                    Reviews = g.Reviews.Select(r => new ReviewDto
                    {
                        ReviewId = r.ReviewId,
                        Note = r.Note,
                        Commentaire = r.Commentaire,
                        UserId = r.UserId
                    }).ToList(),

                    UserGames = g.UserGames.Select(ug => new UserGameDto
                    {
                        UserId = ug.UserId,
                        Username = ug.User.Username,
                        GameId = ug.GameId,
                        GameTitre = ug.Game.Titre,
                        NotePerso = ug.NotePerso,
                        DateAjout = ug.DateAjout,
                    }).ToList()
                    
                }).FirstOrDefaultAsync();
            
            if (game == null)
            {
                return NotFound(new { message = "Jeu non trouvé" });
            }
            
            return Ok(game);
        }
        
        // PUT: api/games/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGame(int id, Game game)
        {
            if (id != game.GameId)
            {
                return BadRequest(new { message = "L'ID de l'URL ne correspond pas à l'ID du jeu" });
            }
            
            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Games.Any(g => g.GameId == id))
                {
                    return NotFound(new { message = "Jeu non trouvé" });
                }
                else
                {
                    throw; 
                }
            }
            
            return NoContent();
        }
        
        // DELETE: api/games/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound(new { message = "Jeu non trouvé" });
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}