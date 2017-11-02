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
            EmpresaCuentas empresaCuentas = new EmpresaCuentas();
            empresaCuentas.Cuentas = db.Cuentas.ToList();
            empresaCuentas.Empresas = db.Empresas.ToList();
            return View(empresaCuentas);
        }


        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile, string EmpresaSeleccionada)
        {
            EmpresaCuentas empresaCuentas = new EmpresaCuentas();
            empresaCuentas.Empresas = db.Empresas.ToList();
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
                            Empresa Empresa = new Empresa();
                            Cuenta cuenta = new Cuenta();
                            db.Entry(cuenta).State = EntityState.Modified;

                            //cuenta.IDEmpresa = Convert.ToInt32(row.Split(',')[0]);
                            string NombreEmpresa = row.Split(',')[0];
                            cuenta.Empresa = db.Empresas.Where(e => e.Nombre == NombreEmpresa).FirstOrDefault();                            
                            if (cuenta.Empresa == null) {
                                TempData["msgExpresionNoValida"] = "<script>alert('Empresa inexistente');</script>";                                
                            }
                            cuenta.Nombre = row.Split(',')[1];
                            cuenta.Fecha = Convert.ToDateTime(row.Split(',')[2]);
                            cuenta.Valor = Convert.ToDecimal(row.Split(',')[3]);
                            db.Cuentas.Add(cuenta);
                            db.SaveChanges();
                        }
                    }
                }               
            }

            if (EmpresaSeleccionada != "")
            {
                int IDEmpresaSeleccionada = Convert.ToInt32(EmpresaSeleccionada);
                empresaCuentas.Cuentas = db.Cuentas
                        .Where(c => c.Empresa.ID == IDEmpresaSeleccionada)
                      .ToList();
            }
            else {
                empresaCuentas.Cuentas = db.Cuentas.ToList();
            }

            return View(empresaCuentas);
            //return RedirectToAction("Index");
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
        public ActionResult Create([Bind(Include = "IDEmpresa,Nombre,Fecha,Valor")] Cuenta cuenta)
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
        public ActionResult Edit([Bind(Include = "IDEmpresa,Nombre,Fecha,Valor")] Cuenta cuenta)
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
