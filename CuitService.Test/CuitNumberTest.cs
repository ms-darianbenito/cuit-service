using Flurl.Http;
using Flurl.Http.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CuitService.Test
{
    public class CuitNumberTest
    {
        [Theory]
        [InlineData(null, "Value cannot be null. (Parameter 'value')", typeof(ArgumentNullException))]
        [InlineData("", "The CUIT number cannot be empty. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("    ", "The CUIT number cannot be empty. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("---", "The CUIT number cannot be empty. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData(" - - - ", "The CUIT number cannot be empty. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20-31111111-8", "The CUIT's verification digit is wrong. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20-31111111-6", "The CUIT's verification digit is wrong. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20-31111111-1", "The CUIT's verification digit is wrong. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20-3111111-8", "The CUIT number must have 11 digits. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20-311111111-6", "The CUIT number must have 11 digits. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("1234a5890", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("1234a", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20 31111111 7", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20x31111111x7", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20,31111111,7", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        [InlineData("20_31111111_7", "The CUIT number cannot have other characters than numbers and dashes. (Parameter 'value')", typeof(ArgumentException))]
        public void Ctor_should_validate_input(string value, string errorMessage, Type exceptionType)
        {
            // Assert
            var exception = (ArgumentException)Assert.Throws(exceptionType, () =>
            {
                // Act
                var cuitNumber = new CuitNumber(value!);
            });

            Assert.Equal(errorMessage, exception.Message);
            Assert.Equal("value", exception.ParamName);
        }

        [Theory]
        [InlineData("20311111117", "20311111117", "20-31111111-7")]
        [InlineData("33123456780", "33123456780", "33-12345678-0")]
        [InlineData("20-31111111-7", "20311111117", "20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0", "33123456780", "33-12345678-0")]
        public void Ctor_should_parse_valid_input(string originalValue, string simplifiedValue, string formattedValue)
        {
            // Act
            var cuitNumber = new CuitNumber(originalValue);

            // Assert
            Assert.Equal(originalValue, cuitNumber.OriginalValue);
            Assert.Equal(simplifiedValue, cuitNumber.SimplifiedValue);
            Assert.Equal(formattedValue, cuitNumber.FormattedValue);
            Assert.Equal(formattedValue, cuitNumber.ToString());
        }

        [Theory]
        [InlineData("20311111117", "20311111117", "20-31111111-7")]
        [InlineData("33123456780", "33123456780", "33-12345678-0")]
        [InlineData("20-31111111-7", "20311111117", "20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0", "33123456780", "33-12345678-0")]
        public void Parse_a_JSON_string_should_create_a_valid_CuitNumber(string originalValue, string simplifiedValue, string formattedValue)
        {
            // Arrange
            var json = $"\"{originalValue}\"";

            // Act
            var cuitNumber = JsonSerializer.Deserialize<CuitNumber>(json);

            // Assert
            Assert.Equal(originalValue, cuitNumber.OriginalValue);
            Assert.Equal(simplifiedValue, cuitNumber.SimplifiedValue);
            Assert.Equal(formattedValue, cuitNumber.FormattedValue);
            Assert.Equal(formattedValue, cuitNumber.ToString());
        }

        [Theory]
        [InlineData("20311111117")]
        [InlineData("00020311111117")]
        public void Parse_a_JSON_number_should_throw_error(string json)
        {
            // Assert
            Assert.Throws<JsonException>(() =>
            {
                // Act
                var cuitNumber = JsonSerializer.Deserialize<CuitNumber>(json);
            });
        }

        [Theory]
        [InlineData("20311111117", "20-31111111-7")]
        [InlineData("33123456780", "33-12345678-0")]
        [InlineData("20-31111111-7", "20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0", "33-12345678-0")]
        public void Stringify_CuitNumber_should_create_a_valid_JSON(string originalValue, string formattedValue)
        {
            // Arrange
            var expectedJson = $"\"{formattedValue}\"";

            // Act
            var cuitNumber = new CuitNumber(originalValue);
            var json = JsonSerializer.Serialize(cuitNumber);

            // Assert
            Assert.Equal(expectedJson, json);
        }

        [Theory]
        [InlineData("20311111117", "20311111117", "20-31111111-7")]
        [InlineData("33123456780", "33123456780", "33-12345678-0")]
        [InlineData("20-31111111-7", "20311111117", "20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0", "33123456780", "33-12345678-0")]
        public async Task GET_with_Flurl_should_parse_a_string_value_as_CuitNumber(string originalValue, string simplifiedValue, string formattedValue)
        {
            // Arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWith($"\"{originalValue}\"");

            // Act
            var cuitNumber = await "http://test.com/give_me_a_cuit".GetJsonAsync<CuitNumber>();

            // Assert
            Assert.Equal(originalValue, cuitNumber.OriginalValue);
            Assert.Equal(simplifiedValue, cuitNumber.SimplifiedValue);
            Assert.Equal(formattedValue, cuitNumber.FormattedValue);
            Assert.Equal(formattedValue, cuitNumber.ToString());
        }

        [Theory]
        [InlineData("20311111117")]
        [InlineData("0020311111117")]
        public async Task GET_with_Flurl_throw_on_parsing_a_number_value_as_CuitNumber(string numberJson)
        {
            // Arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWith(numberJson);

            // Assert
            await Assert.ThrowsAsync<FlurlParsingException>(async () =>
            {
                // Act
                var cuitNumber = await "http://test.com/give_me_a_cuit".GetJsonAsync<CuitNumber>();
            });
        }

        [Theory]
        [InlineData("\"20311111116\"")]
        [InlineData("\"3312345678\"")]
        [InlineData("\"20-31111111-75\"")]
        [InlineData("\"    \"")]
        [InlineData("123")]
        [InlineData("null")]
        [InlineData("false")]
        public async Task GET_with_Flurl_should_deal_with_a_invalid_CuitNumber(string json)
        {
            // Arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(json);

            // Assert
            await Assert.ThrowsAsync<FlurlParsingException>(async () =>
            {
                // Act
                var cuitNumber = await "http://test.com/give_me_a_cuit".GetJsonAsync<CuitNumber>();
            });
        }

        [Theory]
        [InlineData("20311111117", "20-31111111-7")]
        [InlineData("33123456780", "33-12345678-0")]
        [InlineData("20-31111111-7", "20-31111111-7")]
        [InlineData("3-3-1-2-3-4-5-6-7-8-0", "33-12345678-0")]
        public async Task PUT_with_Flurl_should_stringify_a_CuitNumber(string originalValue, string formattedValue)
        {
            // Arrange
            using var httpTest = new HttpTest();
            var expectedJson = $"\"{formattedValue}\"";
            var cuitNumber = new CuitNumber(originalValue);

            // Act
            await "http://test.com/give_me_a_cuit".PostJsonAsync(cuitNumber);

            // Assert
            var httpCallAssertion = httpTest.ShouldHaveMadeACall();
            httpCallAssertion.WithVerb(HttpMethod.Post);
            httpCallAssertion.WithRequestBody(expectedJson);
        }
    }
}
