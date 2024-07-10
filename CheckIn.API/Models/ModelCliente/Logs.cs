using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("Logs")]
    public class Logs
    {
        public int id { get; set; }
        public int idSolicitud { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
    }
}