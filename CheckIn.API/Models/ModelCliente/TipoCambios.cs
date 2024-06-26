using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("TipoCambios")]
    public class TipoCambios
    {
        public int id { get; set; }
        public decimal TipoCambio { get; set; }
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; }
    }
}