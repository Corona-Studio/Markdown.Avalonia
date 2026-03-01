using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Markdown.Avalonia.Extensions
{
    public class ComplementaryExtension : MarkupExtension
    {
        private readonly string _brushName;

        public ComplementaryExtension(string colorKey)
        {
            _brushName = colorKey;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var brush = new DynamicResourceExtension(_brushName).ProvideValue(serviceProvider) as BindingBase
                        ?? new Binding { Source = null };

            return new MultiBinding
            {
                Bindings = new List<BindingBase> { brush },
                Converter = new ComplementaryConverter()
            };
        }

        class ComplementaryConverter : IMultiValueConverter
        {
            public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            {
                if (values.Count < 1)
                    return AvaloniaProperty.UnsetValue;

                Color c;
                if (values[0] is ISolidColorBrush b)
                    c = b.Color;
                else if (values[0] is Color col)
                    c = col;
                else
                    return values[0];

                var rgb = new int[] { c.R, c.G, c.B };
                var s = rgb.Max() + rgb.Min();

                return new SolidColorBrush(
                    Color.FromArgb(
                        c.A,
                        (byte)(s - c.R),
                        (byte)(s - c.G),
                        (byte)(s - c.B)));
            }
        }
    }
}