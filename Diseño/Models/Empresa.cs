using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class Empresa
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Cuenta> Cuentas { get; set; }
    }
}