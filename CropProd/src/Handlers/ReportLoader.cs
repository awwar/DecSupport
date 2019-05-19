using iTextSharp.text.pdf;
using System.Drawing;
using System.IO;

namespace Handlers
{
    class ReportLoader
    {
        string basepath = Path.GetTempPath() + "CropPod/reports/";

        public void MakePDF(Bitmap tile, Bitmap rezult, Bitmap cutout)
        {
            Image img = new Bitmap(tile.Width, tile.Height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.Transparent);
            g.DrawImage(tile, 0, 0, tile.Width, tile.Height);
            g.DrawImage(rezult, 0, 0, rezult.Width, rezult.Height);
            g.DrawImage(cutout, 0, 0, cutout.Width, cutout.Height);

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            using (PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\result.pdf", FileMode.Create)))
            {
                document.Open();

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Png);
                float height = 500;
                float width = img.Width * (height / img.Height);
                pic.ScaleToFit(width, height);
                pic.SetAbsolutePosition(0, document.PageSize.Height - height);


                document.Add(pic);
                iTextSharp.text.Font helvetica = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                BaseFont helveticaBase = helvetica.GetCalculatedBaseFont(false);
                /*writer.DirectContent.BeginText();
                writer.DirectContent.SetFontAndSize(helveticaBase, 12f);
                writer.DirectContent.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, "Hello world!", 35, 766, 0);
                writer.DirectContent.EndText();*/

                document.Close();
                writer.Close();
            }
        }

    }
}
