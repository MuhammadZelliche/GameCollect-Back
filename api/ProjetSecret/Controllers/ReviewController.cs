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
    public class ReviewsController : ControllerBase
    {
        private readonly GameCollectDbContext _context;

        public ReviewsController(GameCollectDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(Review review)
        {
            review.UserId = GetCurrentUserId();
            review.DatePublication = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var dto = new ReviewDto
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                Username = review.User.Username,
                GameId = review.GameId,
                Note = review.Note,
                Commentaire = review.Commentaire,
                DatePublication = review.DatePublication
            };

            return CreatedAtAction(nameof(GetReviewById), new { id = review.ReviewId }, dto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Game)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    GameId = r.GameId,
                    Note = r.Note,
                    Commentaire = r.Commentaire,
                    DatePublication = r.DatePublication
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Game)
                .Where(r => r.ReviewId == id)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    GameId = r.GameId,
                    Note = r.Note,
                    Commentaire = r.Commentaire,
                    DatePublication = r.DatePublication
                })
                .FirstOrDefaultAsync();

            if (review == null)
                return NotFound(new { message = "Review non trouvée" });

            return Ok(review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, Review review)
        {
            if (id != review.ReviewId) return BadRequest();

            var reviewInDb = await _context.Reviews.FindAsync(id);
            if (reviewInDb == null) return NotFound("Review non trouvée");

            var userId = GetCurrentUserId();
            if (reviewInDb.UserId != userId && !User.IsInRole("Admin"))
                return Forbid("Vous n'avez pas les droits de modifier cette review.");

            reviewInDb.Note = review.Note;
            reviewInDb.Commentaire = review.Commentaire;
            reviewInDb.DatePublication = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Review non trouvée");

            var userId = GetCurrentUserId();
            if (review.UserId != userId && !User.IsInRole("Admin"))
                return Forbid("Vous n'avez pas les droits de supprimer cette review.");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
