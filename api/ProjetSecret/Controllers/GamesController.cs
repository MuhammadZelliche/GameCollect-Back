using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Important pour ToListAsync, FindAsync, etc.
using ProjetSecret.Data; // Ton DbContext
using ProjetSecret.Models; // Ton modèle Game
using Microsoft.AspNetCore.Authorization;

namespace ProjetSecret.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")] // -> /api/games
    public class GamesController : ControllerBase
    {
        private readonly GameCollectDbContext _context;

        // Injection du DbContext (comme pour le TestController)
        public GamesController(GameCollectDbContext context)
        {
            _context = context;
        }

        // --- 1. CREATE ---
        // POST: api/games
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Game>> CreateGame(Game game)
        {
            // Ajoute le jeu au "contexte" d'EF Core
            _context.Games.Add(game);
            
            // Applique les changements à la BDD
            await _context.SaveChangesAsync();

            // Renvoie un statut 201 (Created) avec le jeu créé
            // et un en-tête "Location" vers le nouveau jeu (bonne pratique REST)
            return CreatedAtAction(nameof(GetGameById), new { id = game.GameId }, game);
        }

        // --- 2. READ (Tous) ---
        // GET: api/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAllGames()
        {
            // Simple : récupère tous les jeux de la table "Games"
            return await _context.Games.ToListAsync();
        }

        // --- 3. READ (Un seul) ---
        // GET: api/games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(int id)
        {
            // Trouve le jeu par sa clé primaire
            var game = await _context.Games.FindAsync(id);

            // Si non trouvé, renvoie un statut 404 (Not Found)
            if (game == null)
            {
                return NotFound(new { message = "Jeu non trouvé" });
            }

            // Si trouvé, renvoie un statut 200 (Ok) avec le jeu
            return Ok(game);
        }

        // --- 4. UPDATE ---
        // PUT: api/games/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGame(int id, Game game)
        {
            // Vérifie que l'ID de l'URL correspond à l'ID du jeu dans le JSON
            if (id != game.GameId)
            {
                return BadRequest(new { message = "L'ID de l'URL ne correspond pas à l'ID du jeu" });
            }

            // Dit à EF Core de "suivre" cet objet avec l'état "Modifié"
            _context.Entry(game).State = EntityState.Modified;

            try
            {
                // Tente de sauvegarder les changements
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si le jeu n'existe pas (conflit de sauvegarde)
                if (!_context.Games.Any(g => g.GameId == id))
                {
                    return NotFound(new { message = "Jeu non trouvé" });
                }
                else
                {
                    throw; // Lève une autre exception
                }
            }

            // Renvoie un statut 204 (No Content), signalant le succès de la MàJ
            return NoContent();
        }

        // --- 5. DELETE ---
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

            // Renvoie 204 (No Content) pour un succès de suppression
            return NoContent();
        }
    }
}