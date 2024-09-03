﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CheckIn.API.Models.ModelCliente
{
    [Table("CorreosRecepcion")]
    public partial class CorreosRecepcion
    {
        public int id { get; set; }
        [StringLength(500)]
        public string RecepcionEmail { get; set; }

        [StringLength(500)]
        public string RecepcionPassword { get; set; }

        [StringLength(50)]
        public string RecepcionHostName { get; set; }

        public bool? RecepcionUseSSL { get; set; }

        public int RecepcionPort { get; set; }

        public DateTime? RecepcionUltimaLecturaImap { get; set; }
    }
}