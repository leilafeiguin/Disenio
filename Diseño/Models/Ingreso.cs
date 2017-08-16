using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class Ingreso
    {
        public int ID { get; set; }
        public List<Empresa> IdEmpresa { get; set; }
        public int Valor { get; set; }
    }
}