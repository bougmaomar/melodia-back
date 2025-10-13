namespace melodia_api.Models.Account
{
    public class AccountViewDto
    {
        public string Id { get; set; }
		public bool Active { get; set; }
		public string RefreshToken { get; set; }
		public long? AgentId {  get; set; } 	
		public long? ArtistId {  get; set; } 	
		public long? RadioStationId {  get; set; } 	
		public string UserName { get; set; } 	
		public string Email {  get; set; }
		public string PasswordHash { get; set; }
		public string PhoneNumber { get; set; }
        //IsApproved 	
        //RefreshTokenExpiryTime	
        //NormalizedUserName 	
        //NormalizedEmail 	
        //EmailConfirmed 	
        //SecurityStamp 	
        //ConcurrencyStamp 	
        //LastLogin
        //PhoneNumberConfirmed 	
        //TwoFactorEnabled 	
        //LockoutEnd 	
        //LockoutEnabled 	
        //AccessFailedCount
    }
}
