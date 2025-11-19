namespace ProjetSecret.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public required int UserId { get; set; }
        public required string Role { get; set; }
        public required string Username { get; set; }
    }
}
