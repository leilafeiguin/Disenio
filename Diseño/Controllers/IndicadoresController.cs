﻿using System;
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

            if (IndicadorSeleccionado == "ROE") {
                decimal ROE = AplicarROE(indicadorCuenta.Cuentas, EmpresaSeleccionada);
                indicadorCuenta.Cuentas[0].ValorEnIndicador = ROE;
                return View(indicadorCuenta);
            }


            for (int i = 0; i <= indicadorCuenta.Cuentas.Count - 1; i++) {             
                decimal ValorCuentaSeleccionada = indicadorCuenta.Cuentas[i].Valor;
                string FormulaIndicadorSeleccionado = indicadorCuenta.Indicadores[0].Formula;
                indicadorCuenta.Cuentas[i].ValorEnIndicador = evaluarIndicador(FormulaIndicadorSeleccionado, ValorCuentaSeleccionada);
            }
            

            if (IndicadorSeleccionado != null || EmpresaSeleccionada != null)
            {
                return View(indicadorCuenta);
            }
            return RedirectToAction("Index");
        }

        public decimal evaluarIndicador(string FormulaIndicadorSeleccionado, decimal ValorCuentaSeleccionada){
            decimal[] Parametros = { 0, 0 };
            string[] formulaSeparada = FormulaIndicadorSeleccionado.Split();
            char[] Operadores = new char[(formulaSeparada.Length - 1) / 2];
            int numParametro = 0;

            for (int k = 0; k < formulaSeparada.Length; k += 2)
            {
                if (formulaSeparada[k].Contains("{") && formulaSeparada[k].Contains("}"))
                {
                    string IndicadorEnIndicador = formulaSeparada[k].Replace("{", "");
                    IndicadorEnIndicador = IndicadorEnIndicador.Replace("}", "");
                    if (IndicadorEnIndicador == "ValorCuenta")
                    {
                        Parametros[numParametro] = ValorCuentaSeleccionada;
                        numParametro++;
                    }
                    else
                    {//OTRO INDICADOR
                        List<Indicador> OtrosIndicadores = db.Indicadores
                                .Where(c => c.Nombre == IndicadorEnIndicador)
                                .ToList();
                        string FormulaOtroIndicador = OtrosIndicadores[0].Formula;
                        Parametros[numParametro] = evaluarIndicador(FormulaOtroIndicador, ValorCuentaSeleccionada);
                        numParametro++;
                    }
                }
                else
                {
                    Parametros[numParametro] = Convert.ToDecimal(formulaSeparada[k]);
                    numParametro++;
                }
            }
            int numOperador = 0;
            for (int k = 1; k < formulaSeparada.Length; k += 2)
            {

                switch (formulaSeparada[k])
                {
                    case ("+"):
                        Operadores[numOperador] = '+';
                        numOperador++;
                        break;
                    case ("-"):
                        Operadores[numOperador] = '-';
                        numOperador++;
                        break;
                    case ("*"):
                        Operadores[numOperador] = '*';
                        numOperador++;
                        break;
                    case ("/"):
                        Operadores[numOperador] = '/';
                        numOperador++;
                        break;
                }
            }
            return AplicarFormula(Operadores, Parametros);
        }


        public decimal AplicarFormula(char[] Operadores, decimal[] Parametros)
        {
            while (tieneMultiplicacionODivision(Operadores)){
                for (int j = 0; j < Operadores.Length; j++)
                {
                    if (Operadores[j] == '*')
                    {
                        Parametros[j] = (Parametros[j] * Parametros[j + 1]);
                        Parametros = Parametros.Where((source, index) => index != j+1).ToArray();
                        Operadores = Operadores.Where((source, index) => index != j).ToArray();
                    }
                    else if (Operadores[j] == '/')
                    {
                        Parametros[j] = (Parametros[j] / Parametros[j + 1]);
                        Parametros = Parametros.Where((source, index) => index != j+1).ToArray();
                        Operadores = Operadores.Where((source, index) => index != j).ToArray();
                    }
                }            
            }
            
            for (int z = 0; z < Operadores.Length; z++)
            {
                if (Operadores[z] == '+')
                {
                    Parametros[z] = (Parametros[z] + Parametros[z + 1]);
                    Parametros = Parametros.Where((source, index) => index != z+1).ToArray();
                    Operadores = Operadores.Where((source, index) => index != z).ToArray();
                }
                else if (Operadores[z] == '-')
                {
                    Parametros[z] = (Parametros[z] - Parametros[z + 1]);
                    Parametros = Parametros.Where((source, index) => index != z+1).ToArray();
                    Operadores = Operadores.Where((source, index) => index != z).ToArray();
                }
            }
            return Parametros[0];
        }

        bool tieneMultiplicacionODivision(char[] Operadores){
            if (Operadores.Contains('*') || Operadores.Contains('/'))
            {
                return true;
            }
            else {
                return false;
            }
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
            //Evaluar estructurura formula
            Match match = Regex.Match(indicador.Formula, @"({.*} |[0-9]+ )(\+|\-|\*|\/)( {.*}| [0-9]+)(( (\+|\-|\*|\/)( {.*}| [0-9]+))+)?");
            if (match.Success  && ValidarTextoIndicador(indicador.Formula) && (!indicador.Nombre.Contains(' ')))
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
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion de la fórmula o el nombre no es valida. Por favor ingresela de nuevo.');</script>";

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
            //Evaluar estructurura formula
            Match match = Regex.Match(indicador.Formula, @"({.*} |[0-9]+ )(\+|\-|\*|\/)( {.*}| [0-9]+)(( (\+|\-|\*|\/)( {.*}| [0-9]+))+)?");
            if (match.Success && ValidarTextoIndicador(indicador.Formula) && (!indicador.Nombre.Contains(' ')))
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
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion de la fórmula o el nombre no es valida. Por favor ingresela de nuevo.');</script>";

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

        public static decimal AplicarROE(List<Cuenta> Cuentas, string EmpresaSeleccionada) {

            decimal SumatoriaCuentasEmpresa = 0;
            for (int i = 0; i < Cuentas.Count; i++)
            {
                SumatoriaCuentasEmpresa = SumatoriaCuentasEmpresa + Cuentas[i].Valor;
            }

            return (Cuentas[Cuentas.Count - 1].Valor / SumatoriaCuentasEmpresa);
        }
    }


}
