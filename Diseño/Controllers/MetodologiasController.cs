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
using System.Text.RegularExpressions;


namespace Diseño.Controllers
{
    public class MetodologiasController : Controller
    {
        private DondeInviertoContext db = new DondeInviertoContext();

        // GET: Metodologias
        public ActionResult Index()
        {
            TodasLasClases todas = new TodasLasClases();
            todas.Metodologias = db.Metodologias.ToList();
            todas.Empresas = db.Empresas.ToList();
            todas.Cuentas = db.Cuentas.ToList();
            return View(todas);
        }

        [HttpPost]
        public ActionResult Index(string EmpresaSeleccionada, string EmpresaSeleccionada2, string MetodologiaSeleccionada) 
        {
            TodasLasClases todas = new TodasLasClases();
            todas.Metodologias = db.Metodologias.ToList();
            todas.Empresas = db.Empresas.ToList();

            if (EmpresaSeleccionada != "" && EmpresaSeleccionada2 != "" && EmpresaSeleccionada != EmpresaSeleccionada2 && MetodologiaSeleccionada != "")
            {
                int IDEmpresaSeleccionada = Convert.ToInt32(EmpresaSeleccionada);
                int IDEmpresaSeleccionada2 = Convert.ToInt32(EmpresaSeleccionada2);
                int IDMetodologia = Convert.ToInt32(MetodologiaSeleccionada);
                List<Cuenta> CuentasEmpresa1 = new List<Cuenta>();
                List<Cuenta> CuentasEmpresa2 = new List<Cuenta>();

                CuentasEmpresa1 = db.Cuentas
                        .Where(c => c.Empresa.ID == IDEmpresaSeleccionada)
                      .ToList();
                CuentasEmpresa2 = db.Cuentas
                        .Where(c => c.Empresa.ID == IDEmpresaSeleccionada2)
                      .ToList();

                /*if (MetodologiaSeleccionada == "Maximizar ROE") {
                    todas.Cuentas = maximizarRoe(CuentasEmpresa1, CuentasEmpresa2);
                }*/

                string formulaMetodologia = db.Metodologias
                        .Where(c => c.ID == IDMetodologia)
                      .First().Formula;
                todas.Cuentas = evaluarMetodologia(formulaMetodologia, CuentasEmpresa1, CuentasEmpresa2);
                
            }else{
                todas.Cuentas = db.Cuentas.ToList();
            }

            return View(todas);
        }

        public decimal aplicarIndicadorAEmpresa(string unaParte, List<Cuenta> CuentasEmpresa)
        {
            string[] empresaIndicador = unaParte.Split('-');
            string indic = empresaIndicador[1];
            string formulaIndicador = db.Indicadores
                            .Where(c => c.Nombre.Equals(indic))
                          .First().Formula;
            return evaluarIndicador(formulaIndicador, CuentasEmpresa[CuentasEmpresa.Count - 1].Valor, CuentasEmpresa);
        }

        public List<Cuenta> evaluarMetodologia(string FormulaMetologiaSeleccionada, List<Cuenta> CuentasEmpresa1, List<Cuenta> CuentasEmpresa2)
        {
            int empresaActual = 0;
            List<Cuenta>[] idEmpresa = { CuentasEmpresa1, CuentasEmpresa2 };
            decimal[] Parametros = { 0, 0 };
            string[] formulaSeparada = FormulaMetologiaSeleccionada.Split();
            char[] Operadores = new char[(formulaSeparada.Length - 1) / 2];
            int numParametro = 0;

            for (int k = 0; k < formulaSeparada.Length; k += 2)
            {
                if (formulaSeparada[k].Contains("{") && formulaSeparada[k].Contains("}"))
                {
                    string unaParte = GetSubstringByString("{", "}", formulaSeparada[k]);

                    if (existeEmpresaIndicador(unaParte))
                    {
                        Parametros[numParametro] = aplicarIndicadorAEmpresa(unaParte, idEmpresa[empresaActual]);
                        numParametro++;
                        empresaActual++;
                    }
                    else
                    {
                        Parametros[numParametro] = Convert.ToDecimal(formulaSeparada[k]);
                        numParametro++;
                    }
                }
            }
            int numOperador = 0;
            for (int k = 1; k < formulaSeparada.Length; k += 2)
            {

                switch (formulaSeparada[k])
                {
                    case ("<"):
                        Operadores[numOperador] = '<';
                        numOperador++;
                        break;
                    case (">"):
                        Operadores[numOperador] = '>';
                        numOperador++;
                        break;
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
            return AplicarFormulaMetodologia(Operadores, Parametros, CuentasEmpresa1, CuentasEmpresa2);
        }


        public List<Cuenta> AplicarFormulaMetodologia(char[] Operadores, decimal[] Parametros, List<Cuenta> CuentasEmpresa1, List<Cuenta> CuentasEmpresa2)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            for (int z = 0; z < Operadores.Length; z++)
            {
                if (Operadores[z] == '>')
                {
                    if (Parametros[z] > Parametros[z + 1])
                        cuentas = CuentasEmpresa1;
                    else
                        cuentas = CuentasEmpresa2;
                }
                else if (Operadores[z] == '<')
                {
                    if (Parametros[z] < Parametros[z + 1])
                        cuentas = CuentasEmpresa1;
                    else
                        cuentas = CuentasEmpresa2;
                }
            }
            return cuentas;
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

        public static decimal TotalEmpresa(List<Cuenta> Cuentas)
        {
            decimal SumatoriaCuentasEmpresa = 0;
            for (int i = 0; i < Cuentas.Count; i++)
            {
                SumatoriaCuentasEmpresa = SumatoriaCuentasEmpresa + Cuentas[i].Valor;
            }

            return SumatoriaCuentasEmpresa;

        }

        public List<Cuenta> maximizarRoe(List<Cuenta> CuentasEmpresa1, List<Cuenta> CuentasEmpresa2)
        {
            decimal E1 = IndicadoresController.AplicarROE(CuentasEmpresa1);
            //indicadorCuenta1.Cuentas[0].ValorEnIndicador = E1;
            decimal E2 = IndicadoresController.AplicarROE(CuentasEmpresa2);
            //indicadorCuenta2.Cuentas[0].ValorEnIndicador = E2;

            if (E1 > E2)
            {
                return CuentasEmpresa1;
            }
            else
            {
                return CuentasEmpresa2;
            }
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
            //Evaluar estructurura formula
            //Acepta punto: ({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?))+)?
            //Acepta Coma: ({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?))+)?
            Match match = Regex.Match(metodologia.Formula, @"({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+))+)?");
            if (match.Success && ValidarTextoMetodologia(metodologia.Formula))
            {
                if (ModelState.IsValid)
                {
                    db.Metodologias.Add(metodologia);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Devolver mensaje de error, expresion no valida
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion de la fórmula o el nombre no es valida. Por favor ingresela de nuevo.');</script>";

            }

            return View(metodologia);
        }
        //Validar texto metodologia
        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        public bool ValidarTextoMetodologia(string formula)
        {
            //{empresa-ROE} > {empresa-ROA}
            string[] parametros = formula.Split();
            for (int i = 0; i < parametros.Length; i++)
            {
                if (parametros[i].Contains("{") && parametros[i].Contains("}"))
                {
                    string unaParte = GetSubstringByString("{", "}", parametros[i]);
                    
                    if (!existeEmpresaIndicador(unaParte))
                        return false;
                }
            }
            
            return true;
        }

        public bool existeEmpresaIndicador(string unaParte)
        {
            string[] empresaIndicador = unaParte.Split('-');
            string empresa = empresaIndicador[0];
            string indicador = empresaIndicador[1];
            bool empresaOk = false;
            bool indicadorOk = false;
            if (empresa == "empresa")
            {
                empresaOk = true;
            }
            else
            {
                return false;
            }
            List<Indicador> indicadores = new List<Indicador>();
            indicadores = db.Indicadores.ToList();
            foreach (Indicador e in indicadores)
            {
                if (e.Nombre.Equals(indicador))
                {
                    indicadorOk = true;
                }
            }

            return (empresaOk && indicadorOk);
        }

        /*public bool ValidarTextoMetodologia(string formula)
        {
            while (formula.Contains("{"))
            {
                string unaParte = GetSubstringByString("{", "}", formula);
                string aux = "{" + unaParte + "}";
                int index = formula.IndexOf(aux);
                string cleanPath = (index < 0)
                    ? formula
                    : formula.Remove(index, aux.Length);
                //evaluar el substring
                string[] empresaCuenta = unaParte.Split('-');
                string empresaAevaluar = empresaCuenta[0];
                string ceuntaOindicadorAevaluar = empresaCuenta[1];
                //Evaluo la empresa
                List<Empresa> empresas = new List<Empresa>();
                empresas = db.Empresas.ToList();
                int pasoEmpresa = 0;
                foreach (Empresa e in empresas)
                {
                    if (e.Nombre.Equals(empresaAevaluar))
                    {
                        pasoEmpresa = 1;
                    }
                }
                if (pasoEmpresa != 1)
                {
                    return false;
                }
                //Evaluo la cuenta o Indicador
                List<Cuenta> cuentas = new List<Cuenta>();
                cuentas = db.Cuentas.ToList();
                int pasoCuentaIndicador = 0;
                foreach (Cuenta e in cuentas)
                {
                    if (e.Nombre.Equals(ceuntaOindicadorAevaluar))
                    {
                        pasoCuentaIndicador = 1;
                    }
                }
                List<Indicador> indicadores = new List<Indicador>();
                indicadores = db.Indicadores.ToList();
                foreach (Indicador e in indicadores)
                {
                    if (e.Nombre.Equals(ceuntaOindicadorAevaluar))
                    {
                        pasoCuentaIndicador = 1;
                    }
                }
                if (pasoCuentaIndicador != 1)
                {
                    return false;
                }

            }
            return true;
        }*/


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
            //Acepta punto: ({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\.[0-9]+)?))+)?
            //Acepta Coma: ({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+(\,[0-9]+)?))+)?
            Match match = Regex.Match(metodologia.Formula, @"({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+)( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+)((( < | > | == | != | >= | <= )({[A-Za-z0-9]+-[A-Za-z0-9]+}|[0-9]+))+)?");
            if (match.Success && ValidarTextoMetodologia(metodologia.Formula))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(metodologia).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Devolver mensaje de error, expresion no valida
                TempData["msgExpresionNoValida"] = "<script>alert('La expresion de la fórmula o el nombre no es valida. Por favor ingresela de nuevo.');</script>";

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
