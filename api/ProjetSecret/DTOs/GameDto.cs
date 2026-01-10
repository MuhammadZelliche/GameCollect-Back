using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetSecret.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        
        public string Titre { get; set; }
        
        public string Plateforme { get; set; }
        
        public int AnneeSortie { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [MaxLength(50)]
        public string? Rarete { get; set; }
        
        // --- Relations ---
        public ICollection<UserGameDto> UserGames { get; set; } = new List<UserGameDto>();
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }
}

