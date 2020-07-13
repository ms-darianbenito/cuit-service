using AutoFixture;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CuitService.Test
{
    public class TaxInfoApiTest
    {
        [Fact]
        public async Task GET_random_URL_should_return_404_NotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var url = fixture.Create<string>();
            var client = WebApplicationFactoryHelper
                .Create()
                .CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/{url}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_without_a_CUIT_should_return_404_NotFound()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
