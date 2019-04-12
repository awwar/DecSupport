using System;

namespace LatLonToTile
{
    public class TileCoordinate
    {
        int     zoom;
        int     tileSize;
        double  originShift;

        public TileCoordinate(int zoom, int tileSize)
        {
            this.zoom           = zoom;
            this.tileSize       = tileSize;
            this.originShift    = 2 * Math.PI * 6378137 / 2.0;
        }

        public double[] Convert(double lat, double lon, int zoom)
        {
            return LatLonToMeters(lat, lon, zoom);
        }

        private double[] LatLonToMeters(double lat, double lon, int zoom)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);

            my = my * originShift / 180.0;

            double[] rez = MetersToPixels(mx, my, zoom);

            return rez;
        }

        private double[] MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return PixelsToTile(px, py, zoom);
        }

        private double[] PixelsToTile(double px, double py, int zoom)
        {
            double tx = Math.Ceiling(px / tileSize) - 1;
            double ty = Math.Ceiling(py / tileSize) - 1;
            return GoogleTile(tx, ty, zoom);
        }

        private double Resolution(int zoom)
        {
            return (2 * Math.PI * 6378137 / tileSize) / Math.Pow(2, zoom);
        }

        private double[] GoogleTile(double tx, double ty, int zoom)
        {
            return new double[2] { tx, (Math.Pow(2, zoom) - 1) - ty };
        }
    }
}
