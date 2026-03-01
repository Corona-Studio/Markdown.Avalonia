using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Markdown.Avalonia.Extensions
{
    public class MultiplyExtension : MarkupExtension
    {
        private readonly string _resourceKey;
        private readonly double _scale;

        public MultiplyExtension(string resourceKey) : this(resourceKey, 1) { }

        public MultiplyExtension(string resourceKey, double scale)
        {
            _resourceKey = resourceKey;
            _scale = scale;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var resource = new DynamicResourceExtension(_resourceKey).ProvideValue(serviceProvider) as BindingBase
                           ?? new Binding { Source = null };

            return new MultiBinding
            {
                Bindings = new List<BindingBase> { resource },
                Converter = new MultiplyConverter(_scale)
            };
        }

        class MultiplyConverter : IMultiValueConverter
        {
            public double Scale { get; }

            public MultiplyConverter(double scale)
            {
                Scale = scale;
            }

            public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            {
                if (values.Count < 1)
                    return AvaloniaProperty.UnsetValue;

                return values[0] switch
                {
                    short s => (short)(s * Scale),
                    int i => (int)(i * Scale),
                    long l => (long)(l * Scale),
                    float f => (float)(f * Scale),
                    double d => d * Scale,
                    _ => values[0],
                };
            }
        }
    }
}