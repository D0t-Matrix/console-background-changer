using System;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string transferral = "C:\\background-images\\transfer.bmp";
            string filestart = "C:\\background-images\\";
            File.Delete(transferral);
            Random selector = new Random();
            int filechosen = 0;
            filechosen = selector.Next(1, 25);
            filestart = filestart + Convert.ToString(filechosen) + ".bmp";
            File.Copy(filestart, transferral);
            Wallpaper.Set(filestart, Wallpaper.Style.Centered);
            Environment.Exit(0);
        }
    }
    public class Wallpaper
    {
        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        [DllImport("user32.dll")]
        public static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public static bool Set(string filePath, Style style)
        {
            bool Success = false;
            try
            {
                Image i = System.Drawing.Image.FromFile(Path.GetFullPath(filePath));

                Set(i, style);

                Success = true;

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return Success;
        }

        public static bool Set(Image image, Style style)
        {
            bool Success = false;
            try
            {
                string TempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");

                image.Save(TempPath, ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case Style.Stretched:
                        key.SetValue(@"WallpaperStyle", 2.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    case Style.Centered:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    default:
                    case Style.Tiled:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 1.ToString());

                        break;

                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, TempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

                Success = true;

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return Success;
        }
    }
}
