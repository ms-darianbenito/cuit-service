using AutoFixture.Xunit2;
using Flurl.Http.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
namespace CuitService.Test
{
    public class TaxInfoApiTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpTest _httpTest;

        public TaxInfoApiTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpTest = new HttpTest();
        }

        public void Dispose()
        {
            _httpTest.Dispose();
        }

        private readonly object DemoResult = new
        {
            ActividadPrincipal = "620100-SERVICIOS DE CONSULTORES EN INFORMÁTICA Y SUMINISTROS DE PROGRAMAS DE INFORMÁTICA",
            Apellido = (string?)null,
            CUIT = "20-31111111-7",
            CatIVA = "RI",
            CatImpGanancias = "RI",
            DomicilioCodigoPostal = "7600",
            DomicilioDatoAdicional = (string?)null,
            DomicilioDireccion = "CALLE FALSA 123 Piso:2",
            DomicilioLocalidad = "MAR DEL PLATA SUR",
            DomicilioPais = "AR",
            DomicilioProvincia = "01",
            DomicilioTipo = "FISCAL",
            Error = false,
            EstadoCUIT = "ACTIVO",
            Message = (string?)null,
            Monotributo = false,
            MonotributoActividad = (string?)null,
            MonotributoCategoria = (string?)null,
            Nombre = (string?)null,
            PadronData = (string?)null,
            ParticipacionesAccionarias = true,
            PersonaFisica = false,
            RazonSocial = "RZS C.S. SA",
            StatCode = 0
        };

        [Theory]
        [AutoData]
        public async Task GET_random_URL_should_return_404_NotFound(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"https://custom.domain.com/{url}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK_when_JWT_token_is_a_valid_Doppler_PROD_one(string host, string expectedUserName, string expectedPassword)
        {
            // Arrange
            var cuit = "20-31111111-7";
            var expectedUrl = $"http://{host}:33333/api/TaxInfo?ID=20311111117+CardData=Y";

            _httpTest.RespondWithJson(DemoResult);

            using var appFactory = _factory.WithDisabledLifeTimeValidation()
                .AddConfiguration(new Dictionary<string, string>()
                {
                    ["TaxInfoProvider:UseDummyData"] = "false",
                    ["TaxInfoProvider:Host"] = host,
                    ["TaxInfoProvider:UserName"] = expectedUserName,
                    ["TaxInfoProvider:Password"] = expectedPassword
                });
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://custom.domain.com/taxinfo/by-cuit/{cuit}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjg4NDY5LCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOiIxMzY3IiwiY2RoX2N1c3RvbWVySWQiOiIxMzY3Iiwicm9sZSI6IlVTRVIiLCJpYXQiOjE1OTQxNTUwMjYsImV4cCI6MTU5NDE1NjgyNn0.bv-ZHKulKMhBjcftiS-G_xa6MqPd8vmTJLCkitkSzz_lH6OblXnlLSjGAtoViT0yQun_IVqUggdfgY-Qv6cS_YeiYT-EqVLI1KFsFoWtZ7E1Yp5LZuVW70GskwZ7YbV7qlPrOOVBUbt6bD4LtwxudJmIenNBIgIVV-dCTl6vQNXRY65af7Ak1BG8IJxBaPhiFPniMIfNi_6my7NiHtL7Db2eeYgIxXf5_R-8BZFQ0CxWzNDTpdfaB48SnC7n6aEg9FQdOxcu8XX4qPBjGfnvCui2J9s8XgLfRtVQ27WwletL9XnGq79Dyp2PdNUsCcR2d4CMRxvzK1rO2jXSJ9Rf7w");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            var httpCallAssertion = _httpTest.ShouldHaveMadeACall();
            httpCallAssertion.WithVerb(HttpMethod.Get);
            httpCallAssertion.WithUrlPattern(expectedUrl);
            httpCallAssertion.WithHeader("UserName", expectedUserName);
            httpCallAssertion.WithHeader("Password", expectedPassword);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_not_call_backend_when_UseDummyData_is_true()
        {
            // Arrange
            using var appFactory = _factory.WithBypassAuthorization()
                .AddConfiguration(new Dictionary<string, string>()
                {
                    ["TaxInfoProvider:UseDummyData"] = "true"
                });
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            _httpTest.ShouldNotHaveMadeACall();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_should_make_honor_original_format()
        {
            // Arrange
            using var appFactory = _factory.WithBypassAuthorization()
                .AddConfiguration(new Dictionary<string, string>()
                {
                    ["TaxInfoProvider:UseDummyData"] = "true"
                });
            appFactory.Server.PreserveExecutionContext = true;
            var client = appFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            // Validate indentation
            Assert.Matches(@"^{\n\s+""(.+\n)*}$", content);
            // Validate case sensitivity and making honor to model name case
            Assert.Matches(@"""EstadoCUIT"": ""ACTIVO""", content);
            Assert.Matches(@"""CUIT"": ""20-31111111-7""", content);
            // Validate contains null values
            Assert.Matches(@"""Apellido"": null", content);
            // Validate contains false values
            Assert.Matches(@"""Error"": false", content);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_200_OK_when_JWT_token_is_a_valid_Doppler_TEST_one()
        {
            // Arrange
            _httpTest.RespondWithJson(DemoResult);

            var appFactory = _factory.WithDisabledLifeTimeValidation();
            appFactory.Server.PreserveExecutionContext = true;

            var client = appFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjUxMjAxLCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOm51bGwsImNkaF9jdXN0b21lcklkIjpudWxsLCJyb2xlIjoiVVNFUiIsImlhdCI6MTU5NDE1NjIzNSwiZXhwIjoxNTk0MTU4MDM1fQ.gTnvk6vuSrL6QmpgmCljQlurX7LSLEIdTOP383MF0RECoKL-1UD-H9eM8wC8OCPIuDAJPKEX3h3AsxX5UED2qcvwKFVIHSM8PdaqOA4JIYYYOzFwz4S9YwveuX-fy5wnlk1DJS_ZDQxCF6FDKVYCG30u-gZowkS7Bu1SBz9rNVRtK2pceEQ_7vimbIWI_xDu-vDTDnfMPZR0OIjIGcFJ0wjsFNokprf7FPCpk61RYVj-Ydh-pdYOJIUFalVuxpFXNnTG9OBEOp_4xUJ6LZ-FIUtpn5hh8ekd5hLfWVo8zw6HT3CDFNewpPCPE--rmENk9RVFPkbTC3LHydC9PsFGlg");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_taxinfo_by_cuit_with_a_valid_CUIT_should_return_401_Unauthorized_when_no_JWT_token_is_included()
        {
            // Arrange
            var client = _factory.WithDisabledLifeTimeValidation()
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
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/taxinfo/by-cuit/20-31111111-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjg4NDY5LCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOiIxMzY3IiwiY2RoX2N1c3RvbWVySWQiOiIxMzY3Iiwicm9sZSI6IlVTRVIiLCJpYXQiOjE1OTQxNTUwMjYsImV4cCI6MTU5NDE1NjgyNn0.bv-ZHKulKMhBjcftiS-G_xa6MqPd8vmTJLCkitkSzz_lH6OblXnlLSjGAtoViT0yQun_IVqUggdfgY-Qv6cS_YeiYT-EqVLI1KFsFoWtZ7E1Yp5LZuVW70GskwZ7YbV7qlPrOOVBUbt6bD4LtwxudJmIenNBIgIVV-dCTl6vQNXRY65af7Ak1BG8IJxBaPhiFPniMIfNi_6my7NiHtL7Db2eeYgIxXf5_R-8BZFQ0CxWzNDTpdfaB48SnC7n6aEg9FQdOxcu8XX4qPBjGfnvCui2J9s8XgLfRtVQ27WwletL9XnGq79Dyp2PdNUsCcR2d4CMRxvzK1rO2jXSJ9Rf7w");

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
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("https://custom.domain.com/taxinfo/by-cuit/");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
