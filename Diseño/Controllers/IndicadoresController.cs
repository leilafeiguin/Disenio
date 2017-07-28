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
            IndicadorCuenta indicadorCuenta = new IndicadorCuenta();
            indicadorCuenta.Indicadores = db.Indicadores.ToList();
            indicadorCuenta.Cuentas = db.Cuentas.ToList();

            return View(indicadorCuenta);
        }

        [HttpPost]
        public ActionResult Index(string EmpresaSeleccionada, string IndicadorSeleccionado)
        {
            IndicadorCuenta indicadorCuenta = new IndicadorCuenta();
            indicadorCuenta.Indicadores = db.Indicadores.ToList();
            indicadorCuenta.Cuentas = db.Cuentas.ToList();          
            
            if (EmpresaSeleccionada != null)
            {
                indicadorCuenta.Cuentas = db.Cuentas.ToList();
                indicadorCuenta.Cuentas = db.Cuentas
                        .Where(c => c.Empresa == EmpresaSeleccionada)
                      .ToList();
            }

            if (IndicadorSeleccionado != null)
            {
                indicadorCuenta.Indicadores = db.Indicadores.ToList();
                indicadorCuenta.Indicadores = db.Indicadores
                        .Where(c => c.Nombre == IndicadorSeleccionado)
                      .ToList();                
            }

            for (int i = 0; i <= indicadorCuenta.Cuentas.Count - 1; i++) {
                decimal[] Parametros = {0,0};
                decimal ValorCuentaSeleccionada = indicadorCuenta.Cuentas[i].Valor;
                char Operador = '+';
                string FormulaIndicadorSeleccionado = indicadorCuenta.Indicadores[0].Formula;
                if (FormulaIndicadorSeleccionado.Contains('+'))
                {
                    Operador = '+';
                }
                else if (FormulaIndicadorSeleccionado.Contains('-')) {
                    Operador = '-';
                }
                else if (FormulaIndicadorSeleccionado.Contains('*'))
                {
                    Operador = '*';

                }
                else if (FormulaIndicadorSeleccionado.Contains('/'))
                {
                    Operador = '/';
                }

                char[] OperadorChar = {Operador};
                string [] CosasSeparadas = FormulaIndicadorSeleccionado.Split(OperadorChar);
                CosasSeparadas[0] = CosasSeparadas[0].Remove(CosasSeparadas[0].Length - 1);
                CosasSeparadas[1] = CosasSeparadas[1].Remove(0,1);

                for (int j = 0; j <= 1; j++)
                {
                    if (CosasSeparadas[j].Contains('{') && CosasSeparadas[j].Contains('}'))
                    {
                        string IndicadorEnIndicador = GetSubstringByString("{", "}", CosasSeparadas[j]);
                        if (IndicadorEnIndicador == "ValorCuenta")
                        {
                            Parametros[j] = ValorCuentaSeleccionada;
                        }
                        else {//OTRO INDICADOR
                            List<Indicador> OtrosIndicadores = db.Indicadores
                                    .Where(c => c.Nombre == IndicadorEnIndicador)
                                  .ToList();
                            string FormulaOtroIndicador = OtrosIndicadores[0].Formula;
                            Parametros[j] = EvaluarOtroIndicador(FormulaOtroIndicador, ValorCuentaSeleccionada);
                        }
                    }else{
                        Parametros[j] = Convert.ToDecimal(CosasSeparadas[j]);
                    }
                }
                indicadorCuenta.Cuentas[i].ValorEnIndicador = AplicarFormula(Operador, Parametros[0], Parametros[1]);
            }
            

            if (IndicadorSeleccionado != null || EmpresaSeleccionada != null)
            {
                return View(indicadorCuenta);
            }
            return RedirectToAction("Index");
        }

        public decimal EvaluarOtroIndicador(string formula ,decimal valorCuenta) {
            decimal[] Parametros = { 0, 0 };
            char Operador = '+';
            if(formula.Contains('+')){
                Operador = '+';
            }else if(formula.Contains('-')){
                Operador = '-';
            }else if(formula.Contains('/')){
                Operador = '/';
            }else if (formula.Contains('*')){
                Operador = '*';
            }
            char[] OperadorChar = { Operador };
            string[] CosasSeparadas = formula.Split(OperadorChar);
            CosasSeparadas[0] = CosasSeparadas[0].Remove(CosasSeparadas[0].Length - 1);
            CosasSeparadas[1] = CosasSeparadas[1].Remove(0, 1);

            for (int j = 0; j <= 1; j++)
            {
                if (CosasSeparadas[j].Contains('{') && CosasSeparadas[j].Contains('}'))
                {
                    string IndicadorEnIndicador = GetSubstringByString("{", "}", CosasSeparadas[j]);
                    if (IndicadorEnIndicador == "ValorCuenta")
                    {
                        Parametros[j] = valorCuenta;
                    }
                    else
                    {//OTRO INDICADOR
                        List<Indicador> OtrosIndicadores = db.Indicadores
                                .Where(c => c.Nombre == IndicadorEnIndicador)
                              .ToList();
                        string FormulaOtroIndicador = OtrosIndicadores[0].Formula;
                        Parametros[j] = EvaluarOtroIndicador(FormulaOtroIndicador, valorCuenta);
                    }
                }
                else
                {
                    Parametros[j] = Convert.ToDecimal(CosasSeparadas[j]);
                }
            }
            return AplicarFormula(Operador, Parametros[0], Parametros[1]);
        }


        public decimal AplicarFormula(char Operacion, decimal Parametro1, decimal Parametro2)
        {
            switch (Operacion) { 
                case '+':
                    return Parametro1 + Parametro2;
                    break;

                case '-':
                    return Parametro1 - Parametro2;
                    break;

                case '*':
                    return Parametro1 * Parametro2;
                    break;

                case '/':
                    return Parametro1 / Parametro2;
                    break;                    
            }
            return 0;
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
            Match match = Regex.Match(indicador.Formula, @"({.*} |[0-9]+ )(\+|\-|\*|\/)( {.*}| [0-9]+)");
            if (match.Success  && ValidarTextoIndicador(indicador.Formula))
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
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion de la fórmula no es valida. Por favor ingresela de nuevo.');</script>";

            }
            return View(indicador);
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        public bool ValidarTextoIndicador(string formula)
        {
            if (formula.Contains('{') && formula.Contains('}'))
            {
                string IndicadorEnIndicador = GetSubstringByString("{", "}", formula);

                List<Indicador> indicadores = new List<Indicador>();
                indicadores = db.Indicadores.ToList();

                foreach (Indicador i in indicadores)
                {
                    if (i.Nombre.Equals(IndicadorEnIndicador))
                    {
                        return true;
                    }
                }

                if (IndicadorEnIndicador == "ValorCuenta") {
                    return true;
                }
                
            }else{
                return true;
            }
            return false;
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
            Match match = Regex.Match(indicador.Formula, @"({.*} |[0-9]+ )(\+|\-|\*|\/)( {.*}| [0-9]+)");
            if (match.Success && ValidarTextoIndicador(indicador.Formula))
            {
                if (ModelState.IsValid)
                {
                    indicador.Tipo = "Definido";
                    db.Entry(indicador).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Devolver mensaje de error, expresion no valida
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion no es valida. Por favor ingresela de nuevo.');</script>";
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

        public ViewResult CategoryChosen(string MovieType)
        {

            ViewBag.messageString = MovieType;

            return View("Information");

        }
    }


}
