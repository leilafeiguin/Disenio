using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Diseño.Models
{
    public class Cuenta
    {
        public int ID { get; set; }
        public string Empresa { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Valor { get; set; }
    }

    public class CuentaDBContext : DbContext
    {
        public DbSet<Cuenta> Cuentas { get; set; }
    }
}