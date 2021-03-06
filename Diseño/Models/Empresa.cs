﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diseño.Models
{
    public class Empresa
    {
        public Empresa() {
            this.Cuentas = new HashSet<Cuenta>();
        }
        public int ID { get; set; }
        public string Nombre { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public decimal Inversiones { get; set; }

        [NotMapped]
        public decimal Valor { get; set; }

        public virtual ICollection<Cuenta> Cuentas { get; set; }
    }
}