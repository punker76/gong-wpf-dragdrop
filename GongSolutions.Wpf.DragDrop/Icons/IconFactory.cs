using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

        public static Cursor CreateCursor(double rx, double ry, SolidColorBrush brush, Pen pen)
        {
            var vis = new DrawingVisual();
            using (var dc = vis.RenderOpen())
            {
                dc.DrawRectangle(brush, new Pen(Brushes.Black, 0.1), new Rect(0, 0, rx, ry));
                dc.Close();
            }
            var rtb = new RenderTargetBitmap(64, 64, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(vis);

            using (var ms1 = new MemoryStream())
            {
                var penc = new PngBitmapEncoder();
                penc.Frames.Add(BitmapFrame.Create(rtb));
                penc.Save(ms1);

                var pngBytes = ms1.ToArray();
                var size = pngBytes.GetLength(0);

                //.cur format spec http://en.wikipedia.org/wiki/ICO_(file_format)
                using (var ms = new MemoryStream()) {
                    {//ICONDIR Structure
                        ms.Write(BitConverter.GetBytes((Int16)0), 0, 2);//Reserved must be zero; 2 bytes
                        ms.Write(BitConverter.GetBytes((Int16)2), 0, 2);//image type 1 = ico 2 = cur; 2 bytes
                        ms.Write(BitConverter.GetBytes((Int16)1), 0, 2);//number of images; 2 bytes
                    }

                    {//ICONDIRENTRY structure
                        ms.WriteByte(32); //image width in pixels
                        ms.WriteByte(32); //image height in pixels

                        ms.WriteByte(0); //Number of Colors in the color palette. Should be 0 if the image doesn't use a color palette
                        ms.WriteByte(0); //reserved must be 0

                        ms.Write(BitConverter.GetBytes((Int16)(rx / 2.0)), 0, 2);//2 bytes. In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
                        ms.Write(BitConverter.GetBytes((Int16)(ry / 2.0)), 0, 2);//2 bytes. In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top.

                        ms.Write(BitConverter.GetBytes(size), 0, 4);//Specifies the size of the image's data in bytes
                        ms.Write(BitConverter.GetBytes((Int32)22), 0, 4);//Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file
                    }

                    ms.Write(pngBytes, 0, size);//write the png data.
                    ms.Seek(0, SeekOrigin.Begin);
                    return new Cursor(ms);
                }
            }
        }
    }
}
