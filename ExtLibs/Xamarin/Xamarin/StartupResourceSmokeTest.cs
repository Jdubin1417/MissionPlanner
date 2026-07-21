using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace MissionPlanner
{
    public static class StartupResourceSmokeTest
    {
        public static void Run()
        {
            var imageConverter = TypeDescriptor.GetConverter(typeof(Image));
            var iconConverter = TypeDescriptor.GetConverter(typeof(Icon));
            var fontConverter = TypeDescriptor.GetConverter(typeof(Font));

            if (!imageConverter.CanConvertFrom(typeof(byte[])) ||
                !iconConverter.CanConvertFrom(typeof(byte[])) ||
                !fontConverter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException("System.Drawing compatibility converters were not registered.");

            using (var image = imageConverter.ConvertFrom(
                       null, CultureInfo.InvariantCulture, Properties.ResourcesX.splash) as Image)
            {
                if (image == null || image.Width <= 0 || image.Height <= 0)
                    throw new InvalidOperationException("The embedded splash image could not be converted.");
            }

            using (var icon = iconConverter.ConvertFrom(
                       null, CultureInfo.InvariantCulture, Properties.ResourcesX.wizardicon) as Icon)
            {
                if (icon == null || icon.Width <= 0 || icon.Height <= 0)
                    throw new InvalidOperationException("The embedded icon image could not be converted.");
            }

            using (var font = fontConverter.ConvertFrom(
                       null, CultureInfo.InvariantCulture, "Arial, 8pt") as Font)
            {
                if (font == null)
                    throw new InvalidOperationException("The serialized font could not be converted.");
            }
        }
    }
}
