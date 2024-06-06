using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("Solicitudes")]
    public class Solicitudes
    {
        public int id { get; set; }
        public int idUsuarioCreador { get; set; }
        public int idTipoGasto { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaAceptacion { get; set; }
        public decimal Monto { get; set; }
        public string Status { get; set; }
        public string Comentarios { get; set; }
    }
}