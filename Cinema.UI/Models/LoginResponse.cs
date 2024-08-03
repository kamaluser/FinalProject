namespace Cinema.UI.Models
{
    public class LoginResponse
    {
        public TokenDetails Token { get; set; }

        public class TokenDetails
        {
            public string Token { get; set; }
            public bool NeedsPasswordReset { get; set; }
        }
    }
}
