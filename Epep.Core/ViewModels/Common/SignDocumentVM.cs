using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Common
{
    public class SignDocumentVM
    {
        public Guid BlobKey { get; set; }
        public int DocumentType { get; set; }
        public int NewDocumentType { get; set; }
        public string Location { get; set; }
        public string Reason { get; set; }
        /// <summary>
        /// URL to be redirected after signing
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// URL to be redirected after signing
        /// </summary>
        public string ErrorUrl { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// URL to be redirected if user cancel
        /// Must be GET
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// Identifier of PDF to be signed
        /// </summary>
        public string PdfId { get; set; }
        public string PdfUrl { get; set; }

        /// <summary>
        /// Extracted hash to be signed
        /// </summary>
        public string PdfHash { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }
        public long ClientCode { get; set; }


        /// <summary>
        /// PDF Signature
        /// </summary>
        public string Signature { get; set; }
    }
}
