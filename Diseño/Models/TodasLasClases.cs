using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class TodasLasClases
    {
        public List<Indicador> Indicadores { get; set; }
        public List<Empresa> Empresas { get; set; }
        public List<Cuenta> Cuentas { get; set; }
        public List<Metodologia> Metodologias { get; set; }
    }
}