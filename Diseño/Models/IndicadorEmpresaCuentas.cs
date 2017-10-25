using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class IndicadorEmpresaCuentas
    {
        public IndicadorEmpresa IndEmpresas { get; set; }
        public List<Cuenta> Cuentas { get; set; }
    }
}