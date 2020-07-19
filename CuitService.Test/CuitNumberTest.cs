using AutoFixture;
using Flurl.Http.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    }
}
