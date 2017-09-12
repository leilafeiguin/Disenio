using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Diseño.DAL;
using Diseño.Models;
using Diseño.Controllers;

namespace Diseño.Controllers
{
    public class MetodologiasController : Controller
    {
        private DondeInviertoContext db = new DondeInviertoContext();

        // GET: Metodologias
        public ActionResult Index()
        {
            
            MetodologiaCuenta[] arrayMetodologiaCuenta = new MetodologiaCuenta[2];
        
            MetodologiaCuenta metodologiaCuenta = new MetodologiaCuenta();
            metodologiaCuenta.Metodologias = db.Metodologias.ToList();
            metodologiaCuenta.Cuentas = db.Cuentas.ToList();

           
            arrayMetodologiaCuenta[0] = metodologiaCuenta;
            arrayMetodologiaCuenta[1] = metodologiaCuenta;

            return View(arrayMetodologiaCuenta);
        }

        [HttpPost]
        public ActionResult Index(string EmpresaSeleccionada, string EmpresaSeleccionada2, string MetodologiaSeleccionada) {
            int IDEmpresaSeleccionada = Convert.ToInt32(EmpresaSeleccionada);
            int IDEmpresaSeleccionada2 = Convert.ToInt32(EmpresaSeleccionada2);
            MetodologiaCuenta metodologiaCuenta = new MetodologiaCuenta();
            metodologiaCuenta.Metodologias = db.Metodologias.ToList();

            IndicadorCuenta indicadorCuenta1 = new IndicadorCuenta();
            indicadorCuenta1.Cuentas = db.Cuentas
                .Where(c => c.IDEmpresa == IDEmpresaSeleccionada)
                .ToList();
            decimal E1 = IndicadoresController.AplicarROE(indicadorCuenta1.Cuentas, EmpresaSeleccionada);
            //indicadorCuenta1.Cuentas[0].ValorEnIndicador = E1;

            IndicadorCuenta indicadorCuenta2 = new IndicadorCuenta();
            indicadorCuenta2.Cuentas = db.Cuentas
                .Where(c => c.IDEmpresa == IDEmpresaSeleccionada2)
                .ToList();
            decimal E2 = IndicadoresController.AplicarROE(indicadorCuenta2.Cuentas, EmpresaSeleccionada2);
            //indicadorCuenta2.Cuentas[0].ValorEnIndicador = E2;

            if (E1 > E2)
            {
                metodologiaCuenta.Cuentas = indicadorCuenta1.Cuentas;
            }
            else 
            {
                metodologiaCuenta.Cuentas = indicadorCuenta2.Cuentas; 
            }

            MetodologiaCuenta[] arrayMetodologiaCuenta = new MetodologiaCuenta[2];

            MetodologiaCuenta metodologiaCuentaTotal = new MetodologiaCuenta();
            metodologiaCuentaTotal.Metodologias = db.Metodologias.ToList();
            metodologiaCuentaTotal.Cuentas = db.Cuentas.ToList();


            arrayMetodologiaCuenta[0] = metodologiaCuentaTotal;
            arrayMetodologiaCuenta[1] = metodologiaCuenta;

            return View(arrayMetodologiaCuenta);
        }


        // GET: Metodologias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodologia metodologia = db.Metodologias.Find(id);
            if (metodologia == null)
            {
                return HttpNotFound();
            }
            return View(metodologia);
        }

        // GET: Metodologias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Metodologias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nombre,Formula,Descripcion")] Metodologia metodologia)
        {
            if (ModelState.IsValid)
            {
                db.Metodologias.Add(metodologia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(metodologia);
        }

        // GET: Metodologias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodologia metodologia = db.Metodologias.Find(id);
            if (metodologia == null)
            {
                return HttpNotFound();
            }
            return View(metodologia);
        }

        // POST: Metodologias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Formula,Descripcion")] Metodologia metodologia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(metodologia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(metodologia);
        }

        // GET: Metodologias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodologia metodologia = db.Metodologias.Find(id);
            if (metodologia == null)
            {
                return HttpNotFound();
            }
            return View(metodologia);
        }

        // POST: Metodologias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Metodologia metodologia = db.Metodologias.Find(id);
            db.Metodologias.Remove(metodologia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
