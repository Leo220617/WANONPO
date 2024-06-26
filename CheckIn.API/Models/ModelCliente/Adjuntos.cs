using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("Adjuntos")]
    public class Adjuntos
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public int Tipo { get; set; }
        public byte[] Base64 { get; set; }
        public string MimeType { get; set; }
    }
}