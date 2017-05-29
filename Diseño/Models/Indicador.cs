using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class Indicador
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Formula { get; set; }
        public string Tipo { get; set; }
    }
}