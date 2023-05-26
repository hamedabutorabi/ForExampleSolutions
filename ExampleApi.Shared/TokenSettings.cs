namespace ExampleApi.Shared
{
    public class TokenSettings
    {
        public int TokenExpiryInSeconds { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }

    }
}
