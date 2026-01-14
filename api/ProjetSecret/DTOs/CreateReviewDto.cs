using System;

namespace ProjetSecret.DTOs
{
    public class CreateReviewDto
    {
        public int GameId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
    }
}
