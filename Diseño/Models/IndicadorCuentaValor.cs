using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class IndicadorCuentaValor
    {
        public int ID { get; set; }
        public decimal Valor { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual Indicador Indicador { get; set; }
    }
}