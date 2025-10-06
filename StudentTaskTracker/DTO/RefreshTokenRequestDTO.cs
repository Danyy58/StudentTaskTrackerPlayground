namespace UserService.DTO
{
    public class RefreshTokenRequestDTO
    {
        public int ID { get; set; }
        public required string RefreshToken { get; set; }
    }
}
