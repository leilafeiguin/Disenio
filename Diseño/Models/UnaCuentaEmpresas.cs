using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class UnaCuentaEmpresas
    {
        public List<Empresa> Empresas { get; set; }
        public Cuenta Cuenta { get; set; }
    }
}