namespace melodia_api.Models
{
    public class UserInfo
    {
        public string Id { get; set; }
        public long UserId {  get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
