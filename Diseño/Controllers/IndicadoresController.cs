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
            TodasLasClases todas = new TodasLasClases();
            todas.Indicadores = db.Indicadores.ToList();
            todas.Empresas = db.Empresas.ToList();
            todas.Cuentas = db.Cuentas.ToList();
            return View(todas);
        }

        [HttpPost]
        public ActionResult Index(string EmpresaSeleccionada, string IndicadorSeleccionado, Nullable<DateTime> FechaInicial, Nullable<DateTime> FechaFinal)
        {
            TodasLasClases todas = new TodasLasClases();
            todas.Indicadores = db.Indicadores.ToList();
            todas.Empresas = db.Empresas.ToList();
            List<Cuenta> cuentasParaIndicador = new List<Cuenta>();
            List<Cuenta> cuentasDelaEmpresa = new List<Cuenta>();

            //if (EmpresaSeleccionada != null)
            if (EmpresaSeleccionada != "")
            {
                int IDEmpresaSeleccionada = Convert.ToInt32(EmpresaSeleccionada);
                cuentasDelaEmpresa = db.Cuentas
                        .Where(c => c.Empresa.ID == IDEmpresaSeleccionada)
                      .ToList();
            }
            else {
                cuentasDelaEmpresa = db.Cuentas.ToList();
            }

            //Aca filtro por fechas
            if (FechaInicial != null && FechaFinal != null && FechaInicial <= FechaFinal)
            {
                List<Cuenta> cuentasEnFecha = new List<Cuenta>();
                foreach (Cuenta cuentaActual in cuentasDelaEmpresa)
                {
                    if ((cuentaActual.Fecha >= FechaInicial) && (cuentaActual.Fecha <= FechaFinal))
                    {
                        cuentasEnFecha.Add(cuentaActual);
                    }
                }
                todas.Cuentas = cuentasEnFecha;
            }
            else
            {
                todas.Cuentas = cuentasDelaEmpresa;
            }

            //Aca Evaluo los Indicadores
            if (IndicadorSeleccionado != "")
            {
                int IDIndicadorSeleccionado = Convert.ToInt32(IndicadorSeleccionado);
                List<Indicador> indicadorActual = new List<Indicador>();
                indicadorActual = db.Indicadores
                        .Where(c => c.ID == IDIndicadorSeleccionado)
                      .ToList();
                for (int i = 0; i <= todas.Cuentas.Count - 1; i++)
                {
                    decimal ValorCuentaSeleccionada = todas.Cuentas[i].Valor;
                    string FormulaIndicadorSeleccionado = indicadorActual[0].Formula;

                    if (EmpresaSeleccionada != "")
                    {
                        cuentasParaIndicador = todas.Cuentas;
                    }
                    else
                    {
                        int idEmpresaParaIndicador = Convert.ToInt32(todas.Cuentas[i].Empresa.ID);
                        cuentasParaIndicador = db.Cuentas
                            .Where(c => c.Empresa.ID == idEmpresaParaIndicador)
                          .ToList();                        
                    }
                    Cuenta cuentaAux = new Cuenta();
                    cuentaAux = cuentasParaIndicador[i];
                    Indicador indicadorAux = new Indicador();
                    indicadorAux = indicadorActual[0];
                    List<IndicadorCuentaValor> indicadorCuentaValorActual = new List<IndicadorCuentaValor>();
                    indicadorCuentaValorActual = db.IndicadorCuentaValores
                                                .Where(icv => icv.Cuenta.ID == cuentaAux.ID && icv.Indicador.ID == indicadorAux.ID).ToList();

                    if (indicadorCuentaValorActual.Count > 0)
                    {
                        todas.Cuentas[i].ValorConIndicador = indicadorCuentaValorActual[0].Valor;
                    }
                    else {
                        IndicadorCuentaValor indicadorCuentaValor = new IndicadorCuentaValor();
                        todas.Cuentas[i].ValorConIndicador = evaluarIndicador(FormulaIndicadorSeleccionado, ValorCuentaSeleccionada, cuentasParaIndicador);

                        indicadorCuentaValor.Cuenta = cuentasParaIndicador[i];
                        indicadorCuentaValor.Indicador = indicadorActual[0];
                        indicadorCuentaValor.Valor = todas.Cuentas[i].ValorConIndicador;
                        db.IndicadorCuentaValores.Add(indicadorCuentaValor);
                        db.SaveChanges();
                    }
                    
                }
                
            }

            return View(todas);
            //return RedirectToAction("Index");
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
        public ActionResult Create([Bind(Include = "ID,Nombre,Formula,Descripcion,Tipo")] Indicador indicador)
        {
            //Evaluar estructurura formula
            //Acepta punto: ({.*} |[0-9]+(\.[0-9]+)? )(\+|\-|\*|\/)( {.*}| [0-9]+(\.[0-9]+)?)(( (\+|\-|\*|\/)( {.*}| [0-9]+(\.[0-9]+)?))+)?
            //Acepta coma: ({.*} |[0-9]+(\,[0-9]+)? )(\+|\-|\*|\/)( {.*}| [0-9]+(\,[0-9]+)?)(( (\+|\-|\*|\/)( {.*}| [0-9]+(\,[0-9]+)?))+)?
            Match match = Regex.Match(indicador.Formula, @"(^({[A-Za-z0-9]+} |[0-9]+(\,[0-9]+)? )(\+|\-|\*|\/)( {[A-Za-z0-9]+}| [0-9]+(\,[0-9]+)?)(( (\+|\-|\*|\/)( {[A-Za-z0-9]+}| [0-9]+(\,[0-9]+)?))+)?$)");
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

                if (IndicadorEnIndicador == "ValorCuenta" || IndicadorEnIndicador == "TotalEmpresa" || IndicadorEnIndicador == "DeudaEmpresa" || IndicadorEnIndicador == "PatrimonioNeto" || IndicadorEnIndicador == "InversionesEmpresa")
                {
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
        public ActionResult Edit([Bind(Include = "ID,Nombre,Formula,Descripcion,Tipo")] Indicador indicador)
        {
            //Evaluar estructurura formula
            //Acepta punto: ({.*} |[0-9]+(\.[0-9]+)? )(\+|\-|\*|\/)( {.*}| [0-9]+(\.[0-9]+)?)(( (\+|\-|\*|\/)( {.*}| [0-9]+(\.[0-9]+)?))+)?
            //Acepta coma: ({.*} |[0-9]+(\,[0-9]+)? )(\+|\-|\*|\/)( {.*}| [0-9]+(\,[0-9]+)?)(( (\+|\-|\*|\/)( {.*}| [0-9]+(\,[0-9]+)?))+)?
            Match match = Regex.Match(indicador.Formula, @"(^({[A-Za-z0-9]+} |[0-9]+(\,[0-9]+)? )(\+|\-|\*|\/)( {[A-Za-z0-9]+}| [0-9]+(\,[0-9]+)?)(( (\+|\-|\*|\/)( {[A-Za-z0-9]+}| [0-9]+(\,[0-9]+)?))+)?$)");
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

        public decimal evaluarIndicador(string FormulaIndicadorSeleccionado, decimal ValorCuentaSeleccionada, List<Cuenta> Cuentas)
        {
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
                    else if (IndicadorEnIndicador == "TotalEmpresa")
                    {
                        Parametros[numParametro] = TotalEmpresa(Cuentas);
                        numParametro++;
                    }
                    else if (IndicadorEnIndicador == "DeudaEmpresa")
                    {
                        Parametros[numParametro] = DeudaEmpresa(Cuentas);
                        numParametro++;
                    }
                    else if (IndicadorEnIndicador == "PatrimonioNeto")
                    {
                        Parametros[numParametro] = PatrimonioNeto(Cuentas);
                        numParametro++;
                    }
                    else if (IndicadorEnIndicador == "InversionesEmpresa")
                    {
                        Parametros[numParametro] = InversionesEmpresa(Cuentas.First().Empresa);
                        numParametro++;
                    }
                    else
                    {//OTRO INDICADOR
                        List<Indicador> OtrosIndicadores = db.Indicadores
                                .Where(c => c.Nombre == IndicadorEnIndicador)
                                .ToList();
                        string FormulaOtroIndicador = OtrosIndicadores[0].Formula;
                        Parametros[numParametro] = evaluarIndicador(FormulaOtroIndicador, ValorCuentaSeleccionada, Cuentas);
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
            while (tieneMultiplicacionODivision(Operadores))
            {
                for (int j = 0; j < Operadores.Length; j++)
                {
                    if (Operadores[j] == '*')
                    {
                        Parametros[j] = (Parametros[j] * Parametros[j + 1]);
                        Parametros = Parametros.Where((source, index) => index != j + 1).ToArray();
                        Operadores = Operadores.Where((source, index) => index != j).ToArray();
                    }
                    else if (Operadores[j] == '/')
                    {
                        Parametros[j] = (Parametros[j] / Parametros[j + 1]);
                        Parametros = Parametros.Where((source, index) => index != j + 1).ToArray();
                        Operadores = Operadores.Where((source, index) => index != j).ToArray();
                    }
                }
            }

            for (int z = 0; z < Operadores.Length; z++)
            {
                if (Operadores[z] == '+')
                {
                    Parametros[z] = (Parametros[z] + Parametros[z + 1]);
                    Parametros = Parametros.Where((source, index) => index != z + 1).ToArray();
                    Operadores = Operadores.Where((source, index) => index != z).ToArray();
                }
                else if (Operadores[z] == '-')
                {
                    Parametros[z] = (Parametros[z] - Parametros[z + 1]);
                    Parametros = Parametros.Where((source, index) => index != z + 1).ToArray();
                    Operadores = Operadores.Where((source, index) => index != z).ToArray();
                }
            }
            return Parametros[0];
        }

        bool tieneMultiplicacionODivision(char[] Operadores)
        {
            if (Operadores.Contains('*') || Operadores.Contains('/'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal DeudaEmpresa(List<Cuenta> Cuentas)
        {
            decimal SumatoriaDeudaEmpresa = 0;
            for (int i = 0; i < Cuentas.Count; i++)
            {
                SumatoriaDeudaEmpresa = SumatoriaDeudaEmpresa + Cuentas[i].PasivoCirculante;
            }

            return SumatoriaDeudaEmpresa;
        }

        public decimal TotalEmpresa(List<Cuenta> Cuentas)
        {
            return PatrimonioNeto(Cuentas) + DeudaEmpresa(Cuentas);
        }

        public decimal InversionesEmpresa(Empresa empresa)
        {
            return empresa.Inversiones;
        }

        public decimal PatrimonioNeto(List<Cuenta> Cuentas)
        {
            Indicador indicadorActual = new Indicador();
            indicadorActual = db.Indicadores.Where(c => c.Nombre.Equals("IngresoNeto")).First();
            decimal SumatoriaNEtoEmpresa = 0;
            for (int i = 0; i < Cuentas.Count; i++)
            {
                SumatoriaNEtoEmpresa += evaluarIndicador(indicadorActual.Formula, Cuentas[i].Valor, Cuentas);
            }

            return SumatoriaNEtoEmpresa;
        }
    }


}
