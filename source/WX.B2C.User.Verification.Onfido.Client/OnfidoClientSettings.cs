using System;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public class OnfidoClientSettings
    {
        public OnfidoClientSettings(string url, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Value cannot be null or empty.", nameof(token));
            var tokenParts = token.Split(" ");
            if (tokenParts.Length != 2)
                throw new ArgumentException($"Cannot parse {nameof(token)}. It must contains token type and token");

            OnfidoApiUrl = url ?? throw new ArgumentNullException(nameof(url));
            TokenType = tokenParts[0];
            Token = tokenParts[1];
        }

        public string Token { get; }

        public string TokenType { get; }

        public string OnfidoApiUrl { get; }
    }
}
