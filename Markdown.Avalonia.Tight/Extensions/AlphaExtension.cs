using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Markdown.Avalonia.Extensions
{
    public class AlphaExtension : MarkupExtension
    {
        private readonly string _brushName;
        private readonly float _alpha;

        public AlphaExtension(string colorKey) : this(colorKey, 1f) { }

        public AlphaExtension(string colorKey, float alpha)
        {
            _brushName = colorKey;
            _alpha = alpha;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var brush = new DynamicResourceExtension(_brushName).ProvideValue(serviceProvider) as BindingBase
                        ?? new Binding { Source = null };

            return new MultiBinding
            {
                Bindings = new List<BindingBase> { brush },
                Converter = new AlphaConverter(_alpha)
            };
        }

        class AlphaConverter : IMultiValueConverter
        {
            public float Alpha { get; }

            public AlphaConverter(float alpha)
            {
                Alpha = alpha;
            }

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

                return new SolidColorBrush(
                    Color.FromArgb(
                        (byte)(c.A / 255f * Alpha * 255f),
                        c.R, c.G, c.B));
            }
        }
    }
}