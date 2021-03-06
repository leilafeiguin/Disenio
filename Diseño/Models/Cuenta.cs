﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diseño.Models
{
    public class Cuenta
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public decimal Valor { get; set; }
        public decimal PasivoCirculante { get; set; }

        [NotMapped]
        public decimal ValorConIndicador { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}