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

namespace Diseño.Controllers
{
    public class CuentasController : Controller
    {
        private CuentaDBContext db = new CuentaDBContext();

        // GET: Cuentas1
        public ActionResult Index()
        {
            return View(db.Cuentas.ToList());
        }
        

        /*public ActionResult Index(){
            return View(new List<Cuenta>());
        }*/

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
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
            return RedirectToAction("Index");
        }

        /*public JsonResult LlamarJson()
        {
            var output = ObtenerListaCuentas();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        private List<Cuenta> ObtenerListaCuentas()
        {
            List<Cuenta> lCuentas = new List<Cuenta>(){
            new Cuenta(){ ID = 1, Empresa = "Coto",  Fecha = DateTime.Now, Valor = 300 },
            new Cuenta(){ ID = 2, Empresa = "Carrefour",  Fecha = DateTime.Now, Valor = 400 },
            new Cuenta(){ ID = 3, Empresa = "Disco",  Fecha = DateTime.Now, Valor = 200 },
            new Cuenta(){ ID = 4, Empresa = "Dia",  Fecha = DateTime.Now, Valor = 350 },
        };
            return lCuentas;
        }*/


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
