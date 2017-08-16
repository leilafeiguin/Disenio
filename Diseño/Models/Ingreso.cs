using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class Ingreso
    {
        public int ID { get; set; }

        public int Empresa_ID { get; set; }
        [ForeignKey("Empresa_ID")]
        public virtual Empresa Empresa { get; set; }
        public int Valor { get; set; }
    }
}