using Diseño.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.DAL
{
    public class DondeInviertoInitializer : System.Data.Entity. DropCreateDatabaseIfModelChanges<DondeInviertoContext>
    {
        protected override void Seed(DondeInviertoContext context)
        {
            var cuentas = new List<Cuenta>
            {
            new Cuenta{Empresa="Feiguin", Fecha=DateTime.Parse("2005-09-01"), Valor=1000},
            new Cuenta{Empresa="Porracin", Fecha=DateTime.Parse("2005-09-02"), Valor=500},
            new Cuenta{Empresa="Erratchu", Fecha=DateTime.Parse("2005-09-03"), Valor=0},        
            };
            cuentas.ForEach(s => context.Cuentas.Add(s));
            context.SaveChanges();

            var indicadores = new List<Indicador>
            {
            new Indicador{Nombre="Ingreso Neto", Formula="Formula ingreso neto", Tipo="Predefinido"},
            new Indicador{Nombre="Ingreso Bruto", Formula="Formula ingreso bruto", Tipo="Predefinido"}
            };
            indicadores.ForEach(s => context.Indicadores.Add(s));
            context.SaveChanges();

            var empresas = new List<Empresa>
            {
            new Empresa{ID=1, Nombre="Feiguin"},
            new Empresa{ID=2, Nombre="Porracin"},
            new Empresa{ID=3, Nombre="Erratchu"}
            };
           foreach (var item in empresas) {
               context.Empresas.Add(item);
            }

            //empresas.ForEach(s => context.Empresas.Add(s));
            context.SaveChanges();
        }
    }
    
}