using System;

namespace ProjetSecret.DTOs
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int GameId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public DateTime DatePublication { get; set; }
    }
}
