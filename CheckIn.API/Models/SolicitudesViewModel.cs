using CheckIn.API.Models.ModelCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models
{
    public class SolicitudesViewModel
    {
        public int id { get; set; }
        public int idUsuarioCreador { get; set; }
        public int idTipoGasto { get; set; }
        public int idRango { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaAceptacion { get; set; }
        public decimal Monto { get; set; }
        public string Status { get; set; }
        public string Moneda { get; set; }
        public int BaseEntry { get; set; }
        public string Comentarios { get; set; }
        public int idUsuarioAprobador { get; set; }
        public bool Generar { get; set; }
        public List<AdjuntosViewModel> Adjuntos { get; set; }
        public List<FacturasViewModel> Facturas { get; set; }
    }
}