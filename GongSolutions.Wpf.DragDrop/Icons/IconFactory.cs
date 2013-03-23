using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace GongSolutions.Wpf.DragDrop.Icons
{
    /// <summary>
    /// Static class to provide access to standard application images
    /// </summary>
    public static class IconFactory
    {
        public static BitmapImage EffectNone { get { return GetImage("EffectNone.png", 12); } }
        public static BitmapImage EffectCopy { get { return GetImage("EffectCopy.png", 12); } }
        public static BitmapImage EffectMove { get { return GetImage("EffectMove.png", 12); } }
        public static BitmapImage EffectLink { get { return GetImage("EffectLink.png", 12); } }

        /// <summary>
        /// Loads an image based on the name and size required.
        /// Images need to be marked as 'Resource' in the project for them be loaded.
        /// </summary>
        /// <param name="iconName">Name of the icon</param>
        /// <param name="size">The size of the icon</param>
        /// <returns>The image to be displayed</returns>
        private static BitmapImage GetImage(string iconName, int size)
        {
            var uri = new Uri(string.Format(@"pack://application:,,,/GongSolutions.Wpf.DragDrop;component/Icons/{0}", iconName), UriKind.RelativeOrAbsolute);
            var icon = new BitmapImage(uri);
            icon.DecodePixelHeight = size;
            icon.DecodePixelWidth = size;

            return icon;
        }
    }
}
