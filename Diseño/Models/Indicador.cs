using System;
using System.Collections.Generic;
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
    }

    public class IndicadorDBContext : DbContext
    {
        public DbSet<Indicador> Indicadores { get; set; }
    }
}