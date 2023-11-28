using System;
using System.Globalization;
using System.IO;
using System.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace eCase.Components.SummonReportGenerator
{
    public static class SummonDocumentGenerator
    {
        ///<summary>
        /// Get parent of System folder to have Windows folder
        ///</summary>
        private static DirectoryInfo dirWindowsFolder = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System));

        ///<summary>
        /// Concatenate Fonts folder onto Windows folder
        ///</summary>
        private static string strFontsFolder = Path.Combine(dirWindowsFolder.FullName, "Fonts");

        public static byte[] GenerateSummonDocument(SummonDocumentVM vm)
        {
            Font titleFont = FontFactory.GetFont(strFontsFolder + "\\arial.ttf", BaseFont.IDENTITY_H, true, 20, 2);
            Font TextFont = FontFactory.GetFont(strFontsFolder + "\\arial.ttf", BaseFont.IDENTITY_H, true, 15);
            Font BoldTextFont = FontFactory.GetFont(strFontsFolder + "\\arial.ttf", BaseFont.IDENTITY_H, true, 15, 1);

            MemoryStream buffer = new MemoryStream();
            byte[] result = null;

            Document doc = new Document(iTextSharp.text.PageSize.A4, 60, 60, 40, 60);
            PdfWriter wri = PdfWriter.GetInstance(doc, buffer);

            DateTimeFormatInfo bgDateTimeFormat = new CultureInfo("bg-BG", false).DateTimeFormat;

            string dateCreated = vm.DateCreated.ToString("dd.MM.yyyy", bgDateTimeFormat);
            string dateRead = vm.ReadTime.ToString("dd.MM.yyyy", bgDateTimeFormat);
            string readTime = vm.ReadTime.ToString("hh:mm", bgDateTimeFormat);

            var Line1T = new Chunk("Съд: ", TextFont);
            var Line1D = new Chunk(Convert.ToString(vm.CourtName), BoldTextFont);
            Paragraph Line1 = new Paragraph(Line1T);
            Line1.Add(Line1D);
            Line1.SpacingBefore = 60;

            var Line2T = new Chunk("Дело: ", TextFont);
            var Line2D = new Chunk(vm.CaseNumber.ToString(), BoldTextFont);
            Paragraph Line2 = new Paragraph(Line2T);
            Line2.Add(Line2D);

            var Line3T = new Chunk("Вид: ", TextFont);
            var Line3D = new Chunk(vm.CaseKind, BoldTextFont);
            Paragraph Line3 = new Paragraph(Line3T);
            Line3.Add(Line3D);

            var Line4T = new Chunk("Година: ", TextFont);
            var Line4D = new Chunk(vm.CaseYear.ToString(), BoldTextFont);
            Paragraph Line4 = new Paragraph(Line4T);
            Line4.Add(Line4D);

            Paragraph Title = new Paragraph("ОТЧЕТ ЗА ДОСТАВЕНО СЪОБЩЕНИЕ ПО ЕЛЕКТРОНЕН ПЪТ", titleFont);
            Title.Alignment = Element.ALIGN_CENTER;
            Title.SpacingBefore = 200;

            var messageText = new Phrase();
            messageText.Add(new Chunk("          Съобщение тип ", TextFont));
            messageText.Add(new Chunk(vm.SummonKind.ToLower(), BoldTextFont));
            messageText.Add(new Chunk(" по дело номер ", TextFont));
            messageText.Add(new Chunk(vm.CaseNumber.ToString(), BoldTextFont));
            messageText.Add(new Chunk(", издадено на ", TextFont));
            messageText.Add(new Chunk(dateCreated + " г.", BoldTextFont));
            messageText.Add(new Chunk(", е прочетено в електронния портал за достъп до информация до съдебни дела на ", TextFont));
            messageText.Add(new Chunk(dateRead + " г.", BoldTextFont));
            messageText.Add(new Chunk(" в ", TextFont));
            messageText.Add(new Chunk(readTime, BoldTextFont));
            messageText.Add(new Chunk(" и се счита за връчено на ", TextFont));
            messageText.Add(new Chunk(vm.Addressee, BoldTextFont));
            messageText.Add(new Chunk(".", TextFont));

            var paragraph = new Paragraph(messageText);

            paragraph.SpacingBefore = 50;

            doc.Open();

            doc.Add(Line1);
            doc.Add(Line2);
            doc.Add(Line3);
            doc.Add(Line4);
            doc.Add(Title);
            doc.Add(paragraph);

            doc.Close();

            result = AddPagingAndWatermark(buffer.ToArray());

            if (result.Length % 2 != 0)
            {
                var bufferList = result.ToList();
                byte a = 0;

                bufferList.Add(a);
                result = bufferList.ToArray();
            }

            return result;
        }

        public static byte[] AddPagingAndWatermark(byte[] OrgPDF)
        {
            Font PageFooterFont = FontFactory.GetFont(strFontsFolder + "\\arial.ttf", BaseFont.IDENTITY_H, true, 12);

            PdfReader reader = new PdfReader(OrgPDF);
            int Pcount = reader.NumberOfPages;

            MemoryStream outputPdfStream = new MemoryStream();

            PdfStamper stamper = new PdfStamper(reader, outputPdfStream);

            for (int i = 1; i <= Pcount; i++)
            {
                Phrase PageFooter = new Phrase("Страница " + Convert.ToString(i) + " от " + Convert.ToString(Pcount), PageFooterFont);

                PdfContentByte Content = stamper.GetUnderContent(i);
                ColumnText.ShowTextAligned(Content, Element.ALIGN_LEFT, PageFooter, 60, 40, 0);
            }

            stamper.Close();
            return (outputPdfStream.ToArray());
        }
    }
}