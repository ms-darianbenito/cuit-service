using AutoFixture;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CuitService.Test
{
    public class TaxInfoApiTest : IClassFixture<CuitServiceApplicationFactory>
    {
        private readonly CuitServiceApplicationFactory _factory;

        public TaxInfoApiTest(CuitServiceApplicationFactory factory)
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
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_without_a_CUIT_should_return_404_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
