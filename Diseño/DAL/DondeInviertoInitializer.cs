using Diseño.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diseño.DAL
{
    public class DondeInviertoInitializer : System.Data.Entity.CreateDatabaseIfNotExists<DondeInviertoContext>
    {
        protected override void Seed(DondeInviertoContext context)
        {
            var cuentas = new List<Cuenta>
            {
            new Cuenta{IDEmpresa=1, Nombre="Cuenta Septiembre1", Fecha=DateTime.Parse("2005-09-01"), Valor=1000, Empresa=new Empresa{ID=1, Nombre="Feiguin", Fecha=DateTime.Parse("2005-09-04")}},
            new Cuenta{IDEmpresa=2, Nombre="Cuenta Septiembre2", Fecha=DateTime.Parse("2005-09-02"), Valor=500, Empresa= new Empresa{ID=2, Nombre="Porracin", Fecha=DateTime.Parse("2005-09-04")}},
            new Cuenta{IDEmpresa=3, Nombre="Cuenta Septiembre3", Fecha=DateTime.Parse("2005-09-03"), Valor=0, Empresa=new Empresa{ID=3, Nombre="Erratchu", Fecha=DateTime.Parse("2005-09-04")}}, 
            new Cuenta{IDEmpresa=1, Nombre="Cuenta Septiembre4", Fecha=DateTime.Parse("2005-09-04"), Valor=1500, Empresa=new Empresa{ID=1, Nombre="Feiguin", Fecha=DateTime.Parse("2005-09-04")}}       
            };
            cuentas.ForEach(s => context.Cuentas.Add(s));
            context.SaveChanges();

            var indicadores = new List<Indicador>
            {
            new Indicador{Nombre="Ingreso Neto", Formula="{ValorCuenta} * 0.9", Descripcion="Ingreso en mano", Tipo="Predefinido"},
            new Indicador{Nombre="Ingreso Bruto", Formula="{ValorCuenta} * 1", Descripcion="Ingreso con impuestos", Tipo="Predefinido"}
            };
            indicadores.ForEach(s => context.Indicadores.Add(s));
            context.SaveChanges();

            var metodologias = new List<Metodologia>
            {
            new Metodologia{Nombre="Metodologia1", Formula="1 + 2", Descripcion="Test1"},
            new Metodologia{Nombre="Metodologia2", Formula="3 * 1", Descripcion="Test2"}
            };
            metodologias.ForEach(s => context.Metodologias.Add(s));
            context.SaveChanges();

            var empresas = new List<Empresa>
            {
            new Empresa{ID=1, Nombre="Feiguin", Fecha=DateTime.Parse("2005-09-04")},
            new Empresa{ID=2, Nombre="Porracin", Fecha=DateTime.Parse("2005-09-04")},
            new Empresa{ID=3, Nombre="Erratchu", Fecha=DateTime.Parse("2005-09-04")}
            };
           foreach (var item in empresas) {
               context.Empresas.Add(item);
            }

            //empresas.ForEach(s => context.Empresas.Add(s));
            context.SaveChanges();
        }
    }
    
}