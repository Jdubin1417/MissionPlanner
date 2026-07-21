using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    public class IconConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var bytes = value as byte[];
            if (bytes == null)
                return base.ConvertFrom(context, culture, value);

            using (var stream = new MemoryStream(bytes, false))
                return new Icon(stream);
        }
    }

    public class ImageConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var bytes = value as byte[];
            if (bytes == null)
                return base.ConvertFrom(context, culture, value);

            using (var stream = new MemoryStream(bytes, false))
                return Image.FromStream(stream);
        }
    }

    public class FontConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text == null)
                return base.ConvertFrom(context, culture, value);

            text = text.Trim();
            if (text.Length == 0)
                return null;

            culture = culture ?? CultureInfo.CurrentCulture;
            var fields = text.Split(new[] { culture.TextInfo.ListSeparator }, StringSplitOptions.None);
            var familyName = fields[0].Trim();
            var size = 8f;
            var unit = GraphicsUnit.Pixel;

            if (fields.Length > 1)
            {
                var sizeAndUnit = fields[1].Trim();
                string sizeText;
                ParseUnit(sizeAndUnit, out sizeText, out unit);

                if (!float.TryParse(sizeText, NumberStyles.Float, culture, out size) &&
                    !float.TryParse(sizeText, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
                    throw new ArgumentException("Failed to parse font size: " + sizeText, nameof(value));
            }

            var style = FontStyle.Regular;
            for (var index = 2; index < fields.Length; index++)
            {
                var styleText = fields[index];
                if (styleText.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0)
                    style |= FontStyle.Bold;
                if (styleText.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) >= 0)
                    style |= FontStyle.Italic;
                if (styleText.IndexOf("Underline", StringComparison.OrdinalIgnoreCase) >= 0)
                    style |= FontStyle.Underline;
                if (styleText.IndexOf("Strikeout", StringComparison.OrdinalIgnoreCase) >= 0)
                    style |= FontStyle.Strikeout;
            }

            return new Font(familyName, size, style, unit);
        }

        private static void ParseUnit(string value, out string size, out GraphicsUnit unit)
        {
            var units = new[]
            {
                new Unit("display", GraphicsUnit.Display),
                new Unit("world", GraphicsUnit.World),
                new Unit("doc", GraphicsUnit.Document),
                new Unit("pt", GraphicsUnit.Point),
                new Unit("px", GraphicsUnit.Pixel),
                new Unit("in", GraphicsUnit.Inch),
                new Unit("mm", GraphicsUnit.Millimeter)
            };

            foreach (var candidate in units)
            {
                if (!value.EndsWith(candidate.Suffix, StringComparison.OrdinalIgnoreCase))
                    continue;

                size = value.Substring(0, value.Length - candidate.Suffix.Length).Trim();
                unit = candidate.Value;
                return;
            }

            size = value;
            unit = GraphicsUnit.Pixel;
        }

        private struct Unit
        {
            public Unit(string suffix, GraphicsUnit value)
            {
                Suffix = suffix;
                Value = value;
            }

            public string Suffix { get; }
            public GraphicsUnit Value { get; }
        }
    }
}
