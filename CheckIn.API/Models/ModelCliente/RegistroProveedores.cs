using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("RegistroProveedores")]
    public class RegistroProveedores
    {
        public int id { get; set; }
        public string Nombre { get; set; }
        public string NombreComercial { get; set; }
        public string Representante { get; set; }
        public string Telefono { get; set; }
        public string Telefono2 { get; set; }
        public string Regimen { get; set; }
        public string TipoCedula { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string CondicionPago { get; set; }
        public string Moneda { get; set; }
        public string TitularCuenta { get; set; }
        public string CedulaCuenta { get; set; }
        public string Banco { get; set; }
        public string IBANCuentaC { get; set; }
        public string IBANCuentaFC { get; set; }
        public string CuentaBancariaC { get; set; }
        public string CuentaBancariaFC { get; set; }
        public string Detallado { get; set; }
    }
}
