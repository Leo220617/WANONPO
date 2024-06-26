using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models
{
    public class AdjuntosViewModel
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public int Tipo { get; set; }
        public byte[] Base64 { get; set; }
        public string base64Img { get; set; }
        public string MimeType { get; set; }
    }
}