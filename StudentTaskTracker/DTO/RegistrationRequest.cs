namespace UserService.DTO
{
    public class RegistrationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Role { get; set; }
    }
}
