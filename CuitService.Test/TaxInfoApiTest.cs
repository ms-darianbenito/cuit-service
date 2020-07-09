using AutoFixture;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK_when_JWT_token_is_a_valid_Doppler_PROD_one()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .WithDisabledLifeTimeValidation()
                .CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjg4NDY5LCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOiIxMzY3IiwiY2RoX2N1c3RvbWVySWQiOiIxMzY3Iiwicm9sZSI6IlVTRVIiLCJpYXQiOjE1OTQxNTUwMjYsImV4cCI6MTU5NDE1NjgyNn0.a4eVqSBptPJk0y9V5Id1yXEzkSroX7j9712W6HOYzb-9irc3pVFQrdWboHcZPLlbpHUdsuoHmFOU-l14N_CjVF9mwjz0Qp9x88JP2KD1x8YtlxUl4BkIneX6ODQ5q_hDeQX-yIUGoU2-cIXzle-JzRssg-XIbaf34fXnUSiUGnQRAuWg3IkmpeLu9fVSbYrY-qW1os1gBSq4NEESz4T87hJblJv3HWNQFJxAtvhG4MLX2ITm8vYNtX39pwI5gdkLY7bNzWmJ1Uphz1hR-sdCdM2oUWKmRmL7txsoD04w5ca7YbdHQGwCI92We4muOs0-N7a4JHYjuDM9lL_TbJGw2w");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK_when_JWT_token_is_a_valid_Doppler_TEST_one()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .WithDisabledLifeTimeValidation()
                .CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjUxMjAxLCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOm51bGwsImNkaF9jdXN0b21lcklkIjpudWxsLCJyb2xlIjoiVVNFUiIsImlhdCI6MTU5NDE1NjIzNSwiZXhwIjoxNTk0MTU4MDM1fQ.iZ40PoFgqmVXBGGBdUABmewvx6byKXaM9pIkJhdlsbcs9i4TUoXZrC0TaWq3-MrFneuVhOFBXy1n5Entr9_x1JGFu_9hpxuHbh266VvmcqmTDJUO0F3fR2tc-3nwPUQzWSTZC6ArJAdHpnXhB3ysvpZVi22l0dDUOeaHNbrQEkbHc61Zo4RlSU20HQSQQ2NJKw6wUfC3iOznHyTUTLFVlJ4REbTnbOzyUKZYyBKRy_aAseJbKphmT9Lh-mjgVsY3_S6WWHzczhk3eqmb8o8QJ3O_NbQxmHR964aRQutljFv_80cc5A61YbTtgmfoEsu-7HV2FaSZtztk6jesr-3rTg");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_401_Unauthorized_when_no_JWT_token_is_included()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .WithDisabledLifeTimeValidation()
                .CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");

            // Assert
            var authenticateHeader = Assert.Single(response.Headers.WwwAuthenticate);
            Assert.Equal("Bearer", authenticateHeader.Scheme);
            Assert.Null(authenticateHeader.Parameter);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_401_Unauthorized_when_JWT_token_is_an_expired_Doppler_PROD_one()
        {
            // Arrange
            var client = WebApplicationFactoryHelper
                .Create()
                .CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjg4NDY5LCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOiIxMzY3IiwiY2RoX2N1c3RvbWVySWQiOiIxMzY3Iiwicm9sZSI6IlVTRVIiLCJpYXQiOjE1OTQxNTUwMjYsImV4cCI6MTU5NDE1NjgyNn0.a4eVqSBptPJk0y9V5Id1yXEzkSroX7j9712W6HOYzb-9irc3pVFQrdWboHcZPLlbpHUdsuoHmFOU-l14N_CjVF9mwjz0Qp9x88JP2KD1x8YtlxUl4BkIneX6ODQ5q_hDeQX-yIUGoU2-cIXzle-JzRssg-XIbaf34fXnUSiUGnQRAuWg3IkmpeLu9fVSbYrY-qW1os1gBSq4NEESz4T87hJblJv3HWNQFJxAtvhG4MLX2ITm8vYNtX39pwI5gdkLY7bNzWmJ1Uphz1hR-sdCdM2oUWKmRmL7txsoD04w5ca7YbdHQGwCI92We4muOs0-N7a4JHYjuDM9lL_TbJGw2w");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            var authenticateHeader = Assert.Single(response.Headers.WwwAuthenticate);
            Assert.Equal("Bearer", authenticateHeader.Scheme);
            Assert.Contains("error=\"invalid_token\"", authenticateHeader.Parameter);
            Assert.Contains("error_description=\"The token expired at ", authenticateHeader.Parameter);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
