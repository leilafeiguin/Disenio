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
        public ActionResult Index(string MetodologiaSeleccionada)
        {
            TodasLasClases todas = new TodasLasClases();
            todas.Metodologias = db.Metodologias.ToList();

            if (MetodologiaSeleccionada != "")
            {
                int IDMetodologia = Convert.ToInt32(MetodologiaSeleccionada);
                string formulaMetodologia = db.Metodologias
                        .Where(c => c.ID == IDMetodologia)
                      .First().Formula;

                List<Empresa> empresas = new List<Empresa>();
                string[] formulaSeparada = formulaMetodologia.Split('/');
                int idIndicador = Convert.ToInt32(formulaSeparada[0]);
                string formulaIndicador = db.Indicadores
                        .Where(c => c.ID == idIndicador)
                      .First().Formula;
                string operacion = formulaSeparada[1];
                int valor = Convert.ToInt32(formulaSeparada[2]);
                switch (operacion)
                {
                    case ("<"):
                        empresas = Mayor(operacion, valor, formulaIndicador);
                        break;
                    case (">"):
                        empresas = Menor(operacion, valor, formulaIndicador);
                        break;
                    case ("Maximo"):
                        empresas = Maximo(formulaIndicador);
                        break;
                    case ("Minimo"):
                        empresas = Minimo(formulaIndicador);
                        break;
                    case ("Ascendiente"):
                        break;
                    case ("Descendiente"):
                        break;
                }
                todas.Empresas = empresas;

            }
            else
            {
                todas.Empresas = db.Empresas.ToList();
            }

            return View(todas);
        }

        public List<Empresa> Maximo(string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<int> lista = new List<int>();
            empresas = db.Empresas.ToList();
            //Asi es para ordenar Descendiente
            empresas.Sort((empresa1, empresa2) => -1 * maximoConIndicador(empresa1, formulaIndicador).CompareTo(maximoConIndicador(empresa2, formulaIndicador)));
            return empresas;
        }

        public List<Empresa> Minimo(string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<int> lista = new List<int>();
            empresas = db.Empresas.ToList();
            //Asi es para ordenar Ascendiente
            empresas.Sort((empresa1, empresa2) => maximoConIndicador(empresa1, formulaIndicador).CompareTo(maximoConIndicador(empresa2, formulaIndicador)));
            return empresas;
        }

        public decimal maximoConIndicador(Empresa empresa, string formulaIndicador)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = empresa.Cuentas.ToList();
            decimal numero = 0;
            foreach (Cuenta cuentaActual in cuentas)
            {
                decimal valorActual = evaluarIndicador(formulaIndicador, cuentaActual.Valor, cuentas);
                if (valorActual > numero)
                    numero = valorActual;
            }

            return numero;
        }

        public List<Empresa> Mayor(string operacion, decimal valor, string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<int> lista = new List<int>();
            empresas = db.Empresas.ToList();
            //Asi es para ordenar Descendiente
            empresas.Sort((empresa1, empresa2) => -1 * masCuentas(empresa1, formulaIndicador, operacion, valor).CompareTo(masCuentas(empresa2, formulaIndicador, operacion, valor)));
            return empresas;
        }

        public List<Empresa> Menor(string operacion, decimal valor, string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<int> lista = new List<int>();
            empresas = db.Empresas.ToList();
            //Asi es para ordenar Descendiente
            empresas.Sort((empresa1, empresa2) => -1 * masCuentas(empresa1, formulaIndicador, operacion, valor).CompareTo(masCuentas(empresa2, formulaIndicador, operacion, valor)));
            return empresas;
        }

        public decimal masCuentas(Empresa empresa, string formulaIndicador, string operacion, decimal valor)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = empresa.Cuentas.ToList();
            decimal cantidad = 0;
            foreach (Cuenta cuentaActual in cuentas)
            {
                decimal valorActual = evaluarIndicador(formulaIndicador, cuentaActual.Valor, cuentas);
                if (operacion.Equals(">"))
                {
                    if (valorActual > valor)
                        cantidad++;
                }
                if (operacion.Equals("<"))
                {
                    if (valorActual < valor)
                        cantidad++;
                }
            }

            return cantidad;
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
            TodasLasClases todas = new TodasLasClases();
            todas.Indicadores = db.Indicadores.ToList();
            return View(todas);
        }

        // POST: Metodologias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Nombre, int indicador, string operacion, string valor, string Descripcion)
        {
            Metodologia metodologia = new Metodologia();
            metodologia.Nombre = Nombre;
            metodologia.Descripcion = Descripcion;
            metodologia.Formula = indicador + "/" + operacion + "/" + valor;

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
        public ActionResult Edit(string Nombre, int indicador, string operacion, decimal valor, string Descripcion, Metodologia metodologia)
        {
            metodologia.Nombre = Nombre;
            metodologia.Descripcion = Descripcion;
            metodologia.Formula = indicador + "/" + operacion + "/" + valor;

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


    }
}