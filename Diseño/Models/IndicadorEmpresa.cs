using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class IndicadorEmpresa
    {
        public List<Indicador> Indicadores { get; set; }
        public List<Empresa> Empresas { get; set; }
    }
}