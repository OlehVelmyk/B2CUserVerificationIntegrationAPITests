using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class Administrator
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }

        [JsonProperty("token_type")]
        public string TokenType { get; private set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }

        public string Username { get; private set; }

        public DateTime LoggedAt { get; private set; }

        public DateTime ExpiresAt => LoggedAt.AddHours(1);


        public Administrator(string accessToken, string tokenType, string refreshToken)
        {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            TokenType = tokenType ?? throw new ArgumentNullException(nameof(tokenType));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        }

        public static async Task<Administrator> LoginAsync(string tokenUrl, Dictionary<string, string> parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            using var client = new HttpClient();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var admin = JsonConvert.DeserializeObject<Administrator>(raw);

            admin.Username = parameters["username"];
            admin.LoggedAt = DateTime.UtcNow;
            return admin;
        }
    }
}