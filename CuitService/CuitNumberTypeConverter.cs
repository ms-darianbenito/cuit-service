using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CuitService
{
    public class CuitNumberTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return new CuitNumber(stringValue);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            var stringValue = value as string;
            return CuitNumber.ValidateNumber(stringValue) == ValidationResult.Success;
        }
    }
}
