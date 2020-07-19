using Flurl.Http.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CuitService.Test
{
    public class TaxInfoApi_ValidationTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpTest _httpTest;

        public TaxInfoApi_ValidationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpTest = new HttpTest();
        }

        public void Dispose()
        {
            _httpTest.Dispose();
        }

        private static async Task AssertCuitErrorMessage(string expectedErrorMessage, HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var problemDetail = JsonSerializer.Deserialize<JsonElement>(content);
            var title = problemDetail.GetProperty("title").GetString();
            Assert.Equal("One or more validation errors occurred.", title);
            var errors = problemDetail.GetProperty("errors").EnumerateObject();
            Assert.Single(errors);
            var error = errors.First();
            Assert.Equal("cuit", error.Name);
            var messages = error.Value.EnumerateArray();
            Assert.Single(messages);
            var message = messages.First().GetString();
            Assert.Equal(expectedErrorMessage, message);
        }

        [Theory]
        [InlineData("20-31111111-8")]
        [InlineData("20-31111111-6")]
        [InlineData("20-31111111-1")]
        public async Task GET_taxinfo_by_cuit_with_an_invalid_verification_digit_should_return_400_BadRequest(string cuit)
        {
            // Arrange
            var appFactory = _factory.WithBypassAuthorization();
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();

            await AssertCuitErrorMessage("The CUIT's verification digit is wrong.", response);
        }

        [Theory]
        [InlineData("20-3111111-8")]
        [InlineData("20-311111111-6")]
        public async Task GET_taxinfo_by_cuit_with_wrong_length_should_return_400_BadRequest(string cuit)
        {
            // Arrange
            var appFactory = _factory.WithBypassAuthorization();
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();

            await AssertCuitErrorMessage("The CUIT number must have 11 digits.", response);
        }

        [Theory]
        [InlineData("%20%20")]
        [InlineData("%20 %20%20")]
        public async Task GET_taxinfo_by_cuit_with_spaces_should_return_400_BadRequest(string cuit)
        {
            // Arrange
            var client = _factory.WithBypassAuthorization().CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();

            await AssertCuitErrorMessage("The cuit field is required.", response);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("-----")]
        [InlineData("%20%20-")]
        [InlineData("-%20%20-")]
        public async Task GET_taxinfo_by_cuit_with_dashes_or_spaces_should_return_400_BadRequest(string cuit)
        {
            // Arrange
            var appFactory = _factory.WithBypassAuthorization();
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();

            await AssertCuitErrorMessage("The CUIT number cannot be empty.", response);
        }

        [Theory]
        [InlineData("1234a5890")]
        [InlineData("1234a")]
        [InlineData("20 31111111 7")]
        [InlineData("20x31111111x7")]
        [InlineData("20,31111111,7")]
        [InlineData("20_31111111_7")]
        public async Task GET_taxinfo_by_cuit_with_invalid_characters_should_return_400_BadRequest(string cuit)
        {
            // Arrange
            var appFactory = _factory.WithBypassAuthorization();
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();

            await AssertCuitErrorMessage("The CUIT number cannot have other characters than numbers and dashes.", response);
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public async Task GET_taxinfo_by_cuit_without_segment_should_return_404_NotFound(string cuit)
        {
            // Arrange
            var appFactory = _factory.WithBypassAuthorization();
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            _httpTest.ShouldNotHaveMadeACall();
        }


        [Theory]
        [InlineData("20311111117")]
        [InlineData("33123456780")]
        [InlineData("20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0")]
        public async Task GET_taxinfo_by_cuit_should_accept_valid_cuit_numbers(string cuit)
        {
            // Arrange
            _httpTest.RespondWithJson(new { });
            var appFactory = _factory.WithBypassAuthorization()
                .AddConfiguration(new Dictionary<string, string>()
                {
                    ["TaxInfoProvider:UseDummyData"] = "false"
                });
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/taxinfo/by-cuit/{cuit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _httpTest.ShouldHaveMadeACall();
        }
    }
}
