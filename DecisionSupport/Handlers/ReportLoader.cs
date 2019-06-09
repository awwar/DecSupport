using DSCore;
using Models;
using System.Drawing;
using System.IO;

namespace Handlers
{
    class ReportLoader
    {
        string basepath = "";

        public ReportLoader()
        {
            basepath = Settings.TempPath + "CropPod/reports/";

            if (!Directory.Exists(basepath))
            {
                Directory.CreateDirectory(basepath);
            }
        }

        public void MakePDF(Bitmap tile, Bitmap rezult, Bitmap cutout)
        {
            /*System.Drawing.Image img = new Bitmap(tile.Width, tile.Height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.Transparent);
            g.DrawImage(tile, 0, 0, tile.Width, tile.Height);
            g.DrawImage(rezult, 0, 0, rezult.Width, rezult.Height);
            g.DrawImage(cutout, 0, 0, cutout.Width, cutout.Height);

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            using (PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\result.pdf", FileMode.Create)))
            {
                document.Open();
                Image pic = iTextSharp.text.Image.GetInstance(img,iTextSharp.text.BaseColor.BLACK,true);
                float width = writer.PageSize.Width;
                float height = img.Height * (width / img.Width);
                pic.ScaleToFit(width, height);
                pic.SetAbsolutePosition(0, document.PageSize.Height - height);


                document.Add(pic);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                BaseFont helveticaBase = helvetica.GetCalculatedBaseFont(false);
                writer.DirectContent.BeginText();
                writer.DirectContent.SetFontAndSize(helveticaBase, 12f);
                writer.DirectContent.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, "Hello world!", 35, 766, 0);
                writer.DirectContent.EndText();

                document.Close();
                writer.Close();
            }*/
        }

        public Report SaveBitmaps(Bitmap tile, Bitmap rezult, Bitmap cutout)
        {
            Image compile = new Bitmap(tile.Width, tile.Height);
            Graphics g = Graphics.FromImage(compile);
            g.Clear(Color.Transparent);
            g.DrawImage(tile, 0, 0, tile.Width, tile.Height);
            g.DrawImage(rezult, 0, 0, rezult.Width, rezult.Height);
            g.DrawImage(cutout, 0, 0, cutout.Width, cutout.Height);
            if (!Directory.Exists(basepath))
            {
                Directory.CreateDirectory(basepath);
            }
            tile.Save(basepath + "back.png");
            rezult.Save(basepath + "rezult.png");
            compile.Save(basepath + "compile.png");
            cutout.Save(basepath + "Cut.png");
            Report report = new Report()
            {
                Background = tile,
                CutData = rezult,
                CompileData = (Bitmap)compile,
                CutFrame = cutout
            };
            return report;
        }

    }
}
