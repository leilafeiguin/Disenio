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
            MetodologiaCuenta metodologiaCuenta = new MetodologiaCuenta();
            metodologiaCuenta.Metodologias = db.Metodologias.ToList();
            metodologiaCuenta.Cuentas = db.Cuentas.ToList();
            
            return View(metodologiaCuenta);
        }

        [HttpPost]
        public ActionResult Index(string EmpresaSeleccionada, string EmpresaSeleccionada2, string MetodologiaSeleccionada) {

            MetodologiaCuenta metodologiaCuenta = new MetodologiaCuenta();
            metodologiaCuenta.Cuentas = db.Cuentas.ToList();
            metodologiaCuenta.Metodologias = db.Metodologias.ToList();

            IndicadorCuenta indicadorCuenta = new IndicadorCuenta();
            indicadorCuenta.Cuentas = db.Cuentas.ToList();
           

            for (int i = 0; i < metodologiaCuenta.Cuentas.Count; i++) {
                indicadorCuenta.Cuentas = db.Cuentas
                    .Where(c => c.Empresa == indicadorCuenta.Cuentas[i].Empresa)
                    .ToList();
                string empresa = indicadorCuenta.Cuentas[i].Empresa;
                metodologiaCuenta.Cuentas[i].ValorEnIndicador = IndicadoresController.AplicarROE(indicadorCuenta.Cuentas, empresa);
            }

            return View(metodologiaCuenta);

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
        public ActionResult Create([Bind(Include = "ID,Nombre,Declaracion")] Metodologia metodologia)
        {
            if (ModelState.IsValid)
            {
                metodologia.Tipo = "Definido";
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
        public ActionResult Edit([Bind(Include = "ID,Nombre,Declaracion")] Metodologia metodologia)
        {
            if (ModelState.IsValid)
            {
                metodologia.Tipo = "Definido";
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
