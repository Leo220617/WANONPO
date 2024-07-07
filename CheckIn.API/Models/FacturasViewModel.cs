using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models
{
    public class FacturasViewModel
    {
        public int id { get; set; }
        public int idSolicitud { get; set; }
        public string CedulaProveedor { get; set; }
        public string NomProveedor { get; set; }
        public string NumFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentarios { get; set; }
        public string PDF { get; set; }
        public decimal Monto { get; set; }
    }
}