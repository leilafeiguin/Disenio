using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Diseño.Models;
using Diseño.DAL;
using System.Text.RegularExpressions;

namespace Diseño.Controllers
{
    public class IndicadoresController : Controller
    {
        private DondeInviertoContext db = new DondeInviertoContext();

        // GET: Indicadores
        public ActionResult Index()
        {
            return View(db.Indicadores.ToList());
        }

        // GET: Indicadores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadores.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // GET: Indicadores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Indicadores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nombre,Formula")] Indicador indicador)
        {
            Match match = Regex.Match(indicador.Formula, @"({[a-zA-Z]+}|[0-9]+)\+(SUM|RES|MUL|DIV)\+({[a-zA-Z]+}|[0-9]+)");
            if (match.Success)
            {
                if (ModelState.IsValid)
                {
                    indicador.Tipo = "Definido";
                    db.Indicadores.Add(indicador);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Devolver mensaje de error, expresion no valida
                TempData[""] = "La expresion ingresada no es valida";
            }
            return View(indicador);
        }

        // GET: Indicadores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadores.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // POST: Indicadores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Formula")] Indicador indicador)
        {
            Match match = Regex.Match(indicador.Formula, @"({[a-zA-Z]+}|[0-9]+)\+(SUM|RES|MUL|DIV)\+({[a-zA-Z]+}|[0-9]+)");
            if (match.Success)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(indicador).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Devolver mensaje de error, expresion no valida
                TempData[""] = "La expresion ingresada no es valida";
            }
            return View(indicador);
        }

        // GET: Indicadores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadores.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // POST: Indicadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Indicador indicador = db.Indicadores.Find(id);
            db.Indicadores.Remove(indicador);
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
