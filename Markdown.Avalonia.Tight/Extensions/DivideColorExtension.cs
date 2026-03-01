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
    public class DivideColorExtension : MarkupExtension
    {
        private readonly string _frmKey;
        private readonly string _toKey;
        private readonly double _relate;

        public DivideColorExtension(string frm, string to, double relate)
        {
            _frmKey = frm;
            _toKey = to;
            _relate = relate;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            BindingBase left;
            if (Color.TryParse(_frmKey, out var leftColor))
            {
                left = new Binding { Source = leftColor, Mode = BindingMode.OneTime };
            }
            else
            {
                left = new DynamicResourceExtension(_frmKey).ProvideValue(serviceProvider) as BindingBase
                       ?? new Binding { Source = null };
            }

            BindingBase right;
            if (Color.TryParse(_toKey, out var rightColor))
            {
                right = new Binding { Source = rightColor, Mode = BindingMode.OneTime };
            }
            else
            {
                right = new DynamicResourceExtension(_toKey).ProvideValue(serviceProvider) as BindingBase
                        ?? new Binding { Source = null };
            }

            return new MultiBinding
            {
                Bindings = new List<BindingBase> { left, right },
                Converter = new DivideConverter(_relate)
            };
        }
    }

    class DivideConverter : IMultiValueConverter
    {
        public double Relate { get; }

        public DivideConverter(double relate)
        {
            Relate = relate;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                return AvaloniaProperty.UnsetValue;

            Color colL;
            if (values[0] is ISolidColorBrush bl)
                colL = bl.Color;
            else if (values[0] is Color cl)
                colL = cl;
            else
                return values[0];

            Color colR;
            if (values[1] is ISolidColorBrush br)
                colR = br.Color;
            else if (values[1] is Color cr)
                colR = cr;
            else
                return values[0];

            static byte Calc(byte l, byte r, double d)
                => (byte)(l * (1 - d) + r * d);

            return new SolidColorBrush(
                Color.FromArgb(
                    Calc(colL.A, colR.A, Relate),
                    Calc(colL.R, colR.R, Relate),
                    Calc(colL.G, colR.G, Relate),
                    Calc(colL.B, colR.B, Relate)));
        }
    }
}