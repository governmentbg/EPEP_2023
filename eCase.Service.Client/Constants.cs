using System;
using System.Collections.Generic;

namespace eCase.Service.Client
{
    public static class Constants
    {
        public static readonly string HtmlPrivatePath = @"~\..\..\..\TestContent\PrivateContent\TestContent.html";
        public static readonly string DocPrivatePath = @"~\..\..\..\TestContent\PrivateContent\TestContent.doc";
        public static readonly string PdfPrivatePath = @"~\..\..\..\TestContent\PrivateContent\TestContent.pdf";

        public static readonly string HtmlPublicPath = @"~\..\..\..\TestContent\PublicContent\TestContent.html";
        public static readonly string DocPublicPath = @"~\..\..\..\TestContent\PublicContent\TestContent.doc";
        public static readonly string PdfPublicPath = @"~\..\..\..\TestContent\PublicContent\TestContent.pdf";

        public static readonly string HtmlMimeType = "text/html";
        public static readonly string DocMimeType = "application/msword";
        public static readonly string PdfMimeType = "application/pdf";

        public const string EMAIL = "valentin.zmiycharov@gmail.com";
        public const string LAWYER_ID = "e4c37fdd-592f-46d2-84a1-001c40e5f3e6";
        public const string BIRTH_DATE = "1992-12-11";

        public static List<Tuple<string, string>> Docs(bool isPrivate)
        {
            var docs = new List<Tuple<string, string>>();

            if (isPrivate)
            {
                docs.AddRange(
                new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(HtmlPrivatePath, HtmlMimeType),
                    new Tuple<string, string>(DocPrivatePath, DocMimeType),
                    new Tuple<string, string>(PdfPrivatePath, PdfMimeType)
                });
            }
            else
            {
                docs.AddRange(
                new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(HtmlPublicPath, HtmlMimeType),
                    new Tuple<string, string>(DocPublicPath, DocMimeType),
                    new Tuple<string, string>(PdfPublicPath, PdfMimeType)
                });
            }

            return docs;
        }
    }
}
