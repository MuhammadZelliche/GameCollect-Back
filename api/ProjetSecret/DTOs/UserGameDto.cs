using System;

namespace ProjetSecret.DTOs
{
    public class UserGameDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int GameId { get; set; }
        public string GameTitre { get; set; } = null!;
        public int? NotePerso { get; set; }
        public DateTime DateAjout { get; set; }
    }
}
