using Models;
using System.Drawing;

namespace DSCore
{
    public static class Settings
    {
        public static int TileSize { set; get; } = 256;
        public static Image DefaultTileImage { set; get; } = null;
        public static string DistributorName { set; get; } = "";
        public static string DistributorSrc { set; get; } = "";
        public static string TempPath { set; get; } = @"C:\Users\Default\AppData\Local\Temp\";
        public static RezultRules RezPaitRulе { set; get; } = RezultRules.Max;
    }
}
