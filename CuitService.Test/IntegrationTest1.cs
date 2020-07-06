using AutoFixture;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CuitService.Test
{
    public class IntegrationTest1 : IClassFixture<CuitServiceApplicationFactory>
    {
        private readonly CuitServiceApplicationFactory _factory;

        public IntegrationTest1(CuitServiceApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GET_random_URL_should_return_404_NotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var url = fixture.Create<string>();
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/{url}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GET_demo_URL_should_return_200_OK()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/WeatherForecast");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
