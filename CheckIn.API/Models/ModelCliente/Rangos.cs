using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{

    [Table("Rangos")]
    public class Rangos
    {
        public int id { get; set; }
        public int idTipoGasto { get; set; }
        public string Nombre { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public int CantidadAprobaciones { get; set; }
        public bool Activo { get; set; }
    }
}