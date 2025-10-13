using melodia.Entities;

namespace melodia_api.Models.Account
{
    public class AccountCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Artist? Artist { get; set; }
        public Agent? Agent { get; set; }
        public RadioStation? RadioStation { get; set; }
    }
}
