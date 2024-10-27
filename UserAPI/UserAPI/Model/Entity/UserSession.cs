namespace UserAPI.Model.Entity
{
    public class UserSession
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string DeviceInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }
    }
}
