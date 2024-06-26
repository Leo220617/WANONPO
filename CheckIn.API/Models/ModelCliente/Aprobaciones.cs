using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("Aprobaciones")]
    public class Aprobaciones
    {
        public int id { get; set; }
        public int idLogin { get; set; }
        public int idSolicitud { get; set; }
        public DateTime FechaAprobacion { get; set; }
    }
}