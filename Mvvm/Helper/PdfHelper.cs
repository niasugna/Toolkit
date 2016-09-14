using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public static class PdfHelper
    {
        public static bool ToPdfFile(this PlotModel plotModel, string pdfFilePath, int PDF_Width = 800, int PDF_Height = 600)
        {
            using (var streamPng = new MemoryStream())
            {
                try
                {
                    OxyPlot.Wpf.PngExporter.Export(plotModel, streamPng, PDF_Width, PDF_Height, OxyPlot.OxyColor.FromRgb(255, 255, 255), 96);

                    using (var gdi = System.Drawing.Image.FromStream(streamPng))
                    {
                        try
                        {
                            using (PdfSharp.Pdf.PdfDocument pdf = new PdfSharp.Pdf.PdfDocument())
                            {
                                var doc = new PdfSharp.Pdf.PdfPage();

                                using (PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromGdiPlusImage(gdi))
                                {
                                    doc.Width = img.PointWidth * 1.1;
                                    doc.Height = img.PointHeight * 1.2;
                                    pdf.AddPage(doc);
                                    using (PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdf.Pages[0]))
                                    {
                                        xgr.DrawImage(img, 20, 20);
                                    }
                                }
                                pdf.Save(pdfFilePath);

                                pdf.Close();

                                return true;
                            }
                        }
                        catch (Exception err)
                        {
                            Trace.WriteLine(" #### Save PDF error ####" + err.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("#### PngExporter failed ####" + ex.Message);
                }
            }

            return false;
        }

        //Convert PlotModel ---> Pdf  -->  Byte[] 
        public static byte[] ToByteArray(this PlotModel plotModel, int PDF_Width = 800, int PDF_Height = 600)
        {
            MemoryStream streamPdf = new MemoryStream();

            using (var streamImg = new MemoryStream())
            {
                try
                {
                    PngExporter.Export(plotModel, streamImg, PDF_Width, PDF_Height, OxyPlot.OxyColor.FromRgb(255, 255, 255));

                    using (var gdi = System.Drawing.Image.FromStream(streamImg))
                    {
                        try
                        {
                            using (PdfSharp.Pdf.PdfDocument pdf = new PdfSharp.Pdf.PdfDocument())
                            {
                                var doc = new PdfSharp.Pdf.PdfPage();

                                using (PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromGdiPlusImage(gdi))
                                {
                                    doc.Width = img.PointWidth * 1.1;
                                    doc.Height = img.PointHeight * 1.2;
                                    pdf.AddPage(doc);
                                    using (PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdf.Pages[0]))
                                    {
                                        xgr.DrawImage(img, 20, 20);
                                    }
                                }
                                pdf.Save(streamPdf);

                                pdf.Close();
                            }
                        }
                        catch (Exception err)
                        {
                            Trace.WriteLine(" #### Save PDF error ####" + err.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("#### PngExporter failed ####" + ex.Message);
                }
            }
            return streamPdf.ToArray();
        }
    }
}
