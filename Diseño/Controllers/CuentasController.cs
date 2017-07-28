using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Diseño.Models;
using System.IO;
using Diseño.DAL;

namespace Diseño.Controllers
{
    public class CuentasController : Controller
    {
        private DondeInviertoContext db = new DondeInviertoContext();

        // GET: Cuentas1
        public ActionResult Index()
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            List<Cuenta> cuentasTodas = new List<Cuenta>();
            List<Cuenta>[] arrayCuentas = new List<Cuenta>[2];

            cuentasTodas = db.Cuentas.ToList();
            cuentas = db.Cuentas.ToList(); ;
            arrayCuentas[0] = cuentasTodas;
            arrayCuentas[1] = cuentas;
            return View(arrayCuentas);
        }


        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile, string EmpresaSeleccionada)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            List<Cuenta> cuentasTodas = new List<Cuenta>();
            List<Cuenta>[] arrayCuentas = new List<Cuenta>[2];
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath);
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (ModelState.IsValid)
                        {
                            Cuenta cuenta = new Cuenta();
                            db.Entry(cuenta).State = EntityState.Modified;
                            
                            cuenta.Empresa = row.Split(',')[1];
                            cuenta.Fecha = Convert.ToDateTime(row.Split(',')[2]);
                            cuenta.Valor = Convert.ToDecimal(row.Split(',')[3]);
                            db.Cuentas.Add(cuenta);
                            db.SaveChanges();
                        }
                    }
                }               
            }
            if (EmpresaSeleccionada != null)
            {
                cuentasTodas = db.Cuentas.ToList();
                cuentas = db.Cuentas
                        .Where(c => c.Empresa == EmpresaSeleccionada)
                      .ToList();
                arrayCuentas[0] = cuentasTodas;
                arrayCuentas[1] = cuentas;

                return View(arrayCuentas);
            }
            return RedirectToAction("Index");
        }


        // GET: Cuentas1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenta cuenta = db.Cuentas.Find(id);
            if (cuenta == null)
            {
                return HttpNotFound();
            }
            return View(cuenta);
        }

        // GET: Cuentas1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cuentas1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Empresa,Fecha,Valor")] Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                db.Cuentas.Add(cuenta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuenta);
        }

        // GET: Cuentas1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenta cuenta = db.Cuentas.Find(id);
            if (cuenta == null)
            {
                return HttpNotFound();
            }
            return View(cuenta);
        }

        // POST: Cuentas1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Empresa,Fecha,Valor")] Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuenta);
        }

        // GET: Cuentas1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenta cuenta = db.Cuentas.Find(id);
            if (cuenta == null)
            {
                return HttpNotFound();
            }
            return View(cuenta);
        }

        // POST: Cuentas1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cuenta cuenta = db.Cuentas.Find(id);
            db.Cuentas.Remove(cuenta);
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
