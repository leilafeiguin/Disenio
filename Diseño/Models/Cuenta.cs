using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

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

        public virtual Empresa Empresa { get; set; }
    }
}