using System;
using System.ComponentModel;
using System.Globalization;
using NodaTime.Text;

namespace NodaTime.AspNetCore.Mvc.TypeConverters
{
    internal abstract class NodaTimeConverter<T> : TypeConverter
    {
        public sealed override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public sealed override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                var result = Parse(s);

                if (!result.Success)
                {
                    throw new FormatException("Invalid format", result.Exception);
                }

                return result.Value;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public abstract ParseResult<T> Parse(string value);
    }
}