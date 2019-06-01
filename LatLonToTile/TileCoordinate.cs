using System;

namespace LatLonToTile
{
    public class TileCoordinate
    {

        public int[] ConvertWorldToTile(double lat, double lon, int zoom)
        {
            int[] p = new int[2];
            p[0] = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            p[1] = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public double[] ConvertTileToWorld(double tile_x, double tile_y, int zoom)
        {
            double[] p = new double[2];
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p[0] = (tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0;
            p[1] = (180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }
    }
}
