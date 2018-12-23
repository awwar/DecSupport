using Controllers;
using System;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class Form1 : Form
    {
        SceneHandler ScenHand;
        int tileSize = 256;
        double initialResolution = 2 * Math.PI * 6378137 / 256;
        double originShift = 2 * Math.PI * 6378137 / 2.0;
        public Form1()
        {
            InitializeComponent();
            ScenHand = new SceneHandler();
            ScenHand.form = this;
            //Handle draw calls
            scene.Paint += new PaintEventHandler(ScenHand.Draw);
            scene.MouseDown += new MouseEventHandler(ScenHand.Scene_MouseDown);
            //Scene.MouseUp += new MouseEventHandler(ScenHand.Scene_MouseUp);
            scene.MouseMove += new MouseEventHandler(ScenHand.Scene_MouseMoove);

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += GeoWatcherOnStatusChanged;

            _geoWatcher.Start();
        }

        private void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Console.WriteLine(string.Format("Lat: {0}, Long: {1}",
                e.Position.Location.Latitude,
                e.Position.Location.Longitude));
            LatLonToMeters(e.Position.Location.Latitude, e.Position.Location.Longitude,19);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "d:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filename = ofd.FileName;
                    label1.Text = "Открыт файл " + filename;
                }
            }
            Image img = Image.FromFile(filename);
            ScenHand.addFrame(new Vector2(0, 0), img);
            ScenHand.addFrame(new Vector2(0, img.Width), img);
            scene.Refresh();
        }

        private void LatLonToMeters(double lat, double lon, int zoom)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);

            my = my * originShift / 180.0;

            double[] rez = MetersToPixels(mx, my, zoom);
            string path = getimg(rez[0], rez[1], zoom);
            Image img = Image.FromFile(path);
            ScenHand.addFrame(new Vector2(0, 0), img);
            
            path = getimg(rez[0] + 1, rez[1], zoom);
            img = Image.FromFile(path);
            ScenHand.addFrame(new Vector2(256, 0), img);
            
            path = getimg(rez[0] + 1, rez[1] + 1, zoom);
            img = Image.FromFile(path);
            ScenHand.addFrame(new Vector2(256, 256), img);
            
            path = getimg(rez[0], rez[1] + 1, zoom);
            img = Image.FromFile(path);
            ScenHand.addFrame(new Vector2(0, 256), img);


            Console.WriteLine(string.Format("CLat: {0}, CLong: {1}",
                rez[0],
                rez[1]));
        }

        private double[] MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return PixelsToTile(px, py,zoom);
        } 

        private double[] PixelsToTile(double px, double py, int zoom)
        {
            double tx = Math.Ceiling(px / tileSize) - 1;
            double ty = Math.Ceiling(py / tileSize) - 1;
            return GoogleTile(tx,ty, zoom);
        }

        private double Resolution(int zoom )
        {
            return initialResolution / Math.Pow(2,zoom);
        }

        private double[] GoogleTile(double tx, double ty, int zoom)
        {
            return new double[2] { tx, (Math.Pow(2, zoom) - 1) - ty };
        }

        private string getimg(double tx,double ty, int zoom)
        {
            string url = "";
            url = string.Format("https://khms0.googleapis.com/kh?v=821&hl=ru&x={0}&y={1}&z={2}",tx,ty,zoom);
            string baseurl = Path.GetTempPath()+"CropPod\\";
            string filename = tx.ToString()+"_"+ty.ToString()+".jpeg";
            string query = string.Format(@"{0}{1}\", baseurl, zoom);

            Directory.CreateDirectory(query);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                try
                {
                    client.DownloadFile(new Uri(url), query + filename);
                    client.Dispose();
                } 
                catch (Exception e)
                {
                    //Console.Beep();
                }

            }


            return query + filename;
        }

        public void Redraw()
        {
            scene.Invalidate();
        }
    }
}
