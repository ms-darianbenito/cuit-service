using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CuitService
{
    // TODO: move validation inside constructor and ModelState updates inside a modelbinder,
    // see https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-3.1#custom-model-binder-sample
    // TODO: implement IEQualable and IComparable, add JsonConverter and TypeConverter attributes
    // see https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/
    public class CuitNumber
    {
        public string? OriginalValue { get; set; }
        public string SimplifiedValue => OriginalValue?.Replace("-", "") ?? string.Empty;
        // TODO: add a new field Formatted Value, and return that value in ToString

        public static ValidationResult ValidateNumber(string? value)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var simplifiedValue = value.Replace("-", "");
            var error = string.IsNullOrWhiteSpace(simplifiedValue) ? "The CUIT number cannot be empty."
                : !simplifiedValue.All(char.IsNumber) ? "The CUIT number cannot have other characters than numbers and dashes."
                : simplifiedValue.Length != 11 ? "The CUIT number must have 11 digits."
                : !IsVerificationDigitValid(simplifiedValue) ? "The CUIT's verification digit is wrong."
                : null;

            if (error != null)
            {
                return new ValidationResult(error);
            }

            return ValidationResult.Success;
        }


        // Source: https://es.wikipedia.org/wiki/Clave_%C3%9Anica_de_Identificaci%C3%B3n_Tributaria
        private static bool IsVerificationDigitValid(string normalizedCuit)
        {
            var factors = new int[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

            var accumulated = 0;

            for (int i = 0; i < factors.Length; i++)
            {
                accumulated += int.Parse(normalizedCuit[i].ToString()) * factors[i];
            }

            accumulated = 11 - (accumulated % 11);

            if (accumulated == 11)
            {
                accumulated = 0;
            }

            if (int.Parse(normalizedCuit[10].ToString()) != accumulated)
            {
                return false;
            }

            return true;
        }
    }
}
