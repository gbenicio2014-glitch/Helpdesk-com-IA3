using Xunit;
using HelpDeskIA.Api.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HelpDeskIA.Tests {
    public class AuthTests {
        [Fact]
        public void GenerateJwtToken_NotEmpty() {
            var inMemory = new Dictionary<string, string> {
                { "Jwt:Key", "testkeytestkeytestkeytestkey" },
                { "Jwt:Issuer", "Test" },
                { "Jwt:Audience", "Test" },
                { "Jwt:ExpireMinutes", "60" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            var auth = new AuthService(config);
            var token = auth.GenerateJwtToken(1, "user@example.com", "EndUser");
            Assert.False(string.IsNullOrWhiteSpace(token));
        }
    }
}
