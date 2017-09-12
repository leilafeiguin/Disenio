using Diseño.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Diseño.DAL
{
    public class DondeInviertoContext : DbContext
    {

        public DondeInviertoContext() : base("DondeInviertoContext")
        {
        }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Indicador> Indicadores { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Metodologia> Metodologias { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}