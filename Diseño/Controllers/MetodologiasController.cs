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
        private Nullable<DateTime> FechaInicialGlobal = new Nullable<DateTime>();
        private Nullable<DateTime> FechaFinalGlobal = new Nullable<DateTime>();
        private string opcion; 


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
                Metodologia metodologia = db.Metodologias
                        .Where(c => c.ID == IDMetodologia)
                      .First();

                FechaInicialGlobal = metodologia.FechaInicial;
                FechaFinalGlobal = metodologia.FechaFinal;
                List<Empresa> empresas = new List<Empresa>();
                string[] formulaSeparada = metodologia.Formula.Split('/');
                string operacion = formulaSeparada[1];
                decimal valor = Convert.ToDecimal(formulaSeparada[2].Replace(".", ","));
                string formulaIndicador = "";
                if (operacion != "Longevidad")
                {
                    int idIndicador = Convert.ToInt32(formulaSeparada[0]);
                    formulaIndicador = db.Indicadores
                            .Where(c => c.ID == idIndicador)
                          .First().Formula;
                }

                opcion = "";
                if (formulaSeparada.Length == 4)
                    opcion = formulaSeparada[3];

                switch (operacion)
                {
                    case ("Menor"):
                        empresas = MayoroMenor(operacion, valor, formulaIndicador);
                        break;
                    case ("Mayor"):
                        empresas = MayoroMenor(operacion, valor, formulaIndicador);
                        break;
                    case ("Maximo"):
                        empresas = Maximo(formulaIndicador);
                        break;
                    case ("Minimo"):
                        empresas = Minimo(formulaIndicador);
                        break;
                    case ("Longevidad"):
                        empresas = Longevidad(valor);
                        break;
                    case ("Ascendiente"):
                        empresas = CrecienteODecreciente(formulaIndicador, "Creciente");
                        break;
                    case ("Descendiente"):
                        empresas = CrecienteODecreciente(formulaIndicador, "Decreciente");
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

        public List<Empresa> Longevidad(decimal valor)
        {
            List<Empresa> empresas = new List<Empresa>();
            foreach (Empresa empresa in db.Empresas.ToList())
            {
                DateTime Hoy = DateTime.Today;
                decimal años = Hoy.Year - valor;
                if (años >= empresa.Fecha.Year)
                    empresas.Add(empresa);
            }
            //Asi es para ordenar Descendiente
            empresas.Sort((empresa1, empresa2) => -1 * empresa1.Fecha.CompareTo(empresa2.Fecha));
            return empresas;
        }

        public List<Empresa> CrecienteODecreciente(string formulaIndicador, string crecienteODecreciente)
        {
            List<Empresa> empresas = new List<Empresa>();
            foreach (Empresa empresa in db.Empresas.ToList())
            {
                if (EmpresaEsCrecienteODecreciente(empresa, formulaIndicador, crecienteODecreciente))
                    empresas.Add(empresa);
            }
            return empresas;

        }

        public bool EmpresaEsCrecienteODecreciente(Empresa empresa, string formulaIndicador, string crecienteODecreciente)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = filtrarPorFecha(empresa.Cuentas.ToList());
            decimal numero = 0;
            if (crecienteODecreciente.Equals("Decreciente"))
                numero = 9999999;
            for (int i = 0; i < cuentas.Count; i++)
            {
                decimal valorActual = evaluarIndicador(formulaIndicador, cuentas[i].Valor, cuentas);
                if (crecienteODecreciente.Equals("Creciente"))
                {
                    if (valorActual < numero)
                        return false;
                }
                else
                {
                    if (valorActual > numero)
                        return false;
                }
                numero = valorActual;
            }
            return true;
        }

        public List<Empresa> Maximo(string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            empresas = db.Empresas.ToList();
            foreach (Empresa empresa in empresas)
            {
                empresa.Valor = maximoOMinimoConIndicador(empresa, formulaIndicador, "Maximo");
            }
            //Asi es para ordenar Descendiente
            empresas.Sort((empresa1, empresa2) => -1 * empresa1.Valor.CompareTo(empresa2.Valor));
            return empresas;
        }

        public List<Empresa> Minimo(string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            empresas = db.Empresas.ToList();
            foreach (Empresa empresa in empresas)
            {
                empresa.Valor = maximoOMinimoConIndicador(empresa, formulaIndicador, "Minimo");
            }
            //Asi es para ordenar Ascendiente
            empresas.Sort((empresa1, empresa2) => empresa1.Valor.CompareTo(empresa2.Valor));
            return empresas;
        }

        public decimal maximoOMinimoConIndicador(Empresa empresa, string formulaIndicador, string maximoOMinimo)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = filtrarPorFecha(empresa.Cuentas.ToList());
            decimal numero = 0;
            if (maximoOMinimo.Equals("Minimo"))
                numero = 9999999;

            foreach (Cuenta cuentaActual in cuentas)
            {
                decimal valorActual = evaluarIndicador(formulaIndicador, cuentaActual.Valor, cuentas);
                if (maximoOMinimo.Equals("Maximo"))
                {
                    if (valorActual > numero)
                        numero = valorActual;
                }
                else
                {
                    if (valorActual < numero)
                        numero = valorActual;
                }
            }

            return numero;
        }

        public List<Empresa> MayoroMenor(string operacion, decimal valor, string formulaIndicador)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<Empresa> empresasRetornar = new List<Empresa>();
            decimal comparador = 0;
            List<int> lista = new List<int>();
            empresas = db.Empresas.ToList();
            foreach (Empresa empresa in empresas)
            {
                if(opcion != ""){
                    comparador = obtenerTotalConIndicador(empresa, formulaIndicador, operacion,valor);
                }else{
                    comparador = masCuentas(empresa, formulaIndicador, operacion, valor);
                }
                if (comparador != 0)
                {
                    empresa.Valor = comparador;
                    empresasRetornar.Add(empresa);
                }
            }

            if (operacion.Equals("Mayor"))
            {
                empresasRetornar.Sort((empresa1, empresa2) => -1 * empresa1.Valor.CompareTo(empresa2.Valor));
            }
            if (operacion.Equals("Menor"))
            {
                empresasRetornar.Sort((empresa1, empresa2) => empresa1.Valor.CompareTo(empresa2.Valor));
            }

            return empresasRetornar;
        }

        public decimal masCuentas(Empresa empresa, string formulaIndicador, string operacion, decimal valor)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = filtrarPorFecha(empresa.Cuentas.ToList());
            decimal numero = 0;
            foreach (Cuenta cuentaActual in cuentas)
            {
                decimal valorActual = evaluarIndicador(formulaIndicador, cuentaActual.Valor, cuentas);
                if (operacion.Equals("Mayor"))
                {
                    if (valorActual > valor)
                        numero = valorActual;
                }
                if (operacion.Equals("Menor"))
                {
                    if (valorActual < valor)
                        numero = valorActual;
                }
            }

            return numero;
        }

        public decimal obtenerTotalConIndicador(Empresa empresa, string formulaIndicador, string operacion, decimal valor)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            cuentas = filtrarPorFecha(empresa.Cuentas.ToList());
            decimal numero = 0;
            decimal parcial = 0;
            decimal total = 0;
            foreach (Cuenta cuentaActual in cuentas)
            {
                parcial = evaluarIndicador(formulaIndicador, cuentaActual.Valor, cuentas);
                total += parcial;
                cuentaActual.ValorConIndicador = parcial;
            }

            //Ordeno ascendiente por si es mediana
            cuentas.Sort((cuenta1, cuenta2) => cuenta1.ValorConIndicador.CompareTo(cuenta2.Valor));

            if (opcion.Equals("Promedio"))
                total = total / cuentas.Count;

            if (opcion.Equals("Mediana"))
            { 
                //Si es par
                if(cuentas.Count % 2 == 0)
                    total = (cuentas[cuentas.Count / 2].ValorConIndicador + cuentas[(cuentas.Count / 2) - 1].ValorConIndicador) / 2;
                else
                    total = cuentas[(cuentas.Count - 1) / 2].ValorConIndicador;
            }

            if (operacion.Equals("Mayor"))
            {
                if (total > valor)
                    numero = total;
            }
            if (operacion.Equals("Menor"))
            {
                if (total < valor)
                    numero = total;
            }

            return numero;
        }

       public List<Cuenta> filtrarPorFecha(List<Cuenta> cuentasDelaEmpresa)
        {
            //Aca filtro por fechas
            if (FechaInicialGlobal != null && FechaFinalGlobal != null && FechaInicialGlobal <= FechaFinalGlobal)
            {
                List<Cuenta> cuentasEnFecha = new List<Cuenta>();
                foreach (Cuenta cuentaActual in cuentasDelaEmpresa)
                {
                    if ((cuentaActual.Fecha >= FechaInicialGlobal) && (cuentaActual.Fecha <= FechaFinalGlobal))
                    {
                        cuentasEnFecha.Add(cuentaActual);
                    }
                }
                return cuentasEnFecha;
            }
            else
            {
                return cuentasDelaEmpresa;
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
        public ActionResult Create(string Nombre, int indicador, string operacion, string valor, string Descripcion, string Opcion, Nullable<DateTime> Inicial, Nullable<DateTime> Final)
        {
            Metodologia metodologia = new Metodologia();
            metodologia.Nombre = Nombre;
            metodologia.Descripcion = Descripcion;
            if (Opcion != "")
                metodologia.Formula = indicador + "/" + operacion + "/" + valor + "/" + Opcion;
            else
                metodologia.Formula = indicador + "/" + operacion + "/" + valor;
            metodologia.FechaInicial = Inicial;
            metodologia.FechaFinal = Final;

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
            List<Metodologia> Metodologias = new List<Metodologia>();
            TodasLasClases todas = new TodasLasClases();
            todas.Indicadores = db.Indicadores.ToList();
            Metodologias.Add(metodologia);
            todas.Metodologias = Metodologias;
            return View(todas);
        }

        // POST: Metodologias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Nombre, int indicador, string operacion, string valor, string Descripcion, string Opcion, Nullable<DateTime> Inicial, Nullable<DateTime> Final, Metodologia metodologia)
        {
            Metodologia metodologiaa = db.Metodologias.Find(metodologia.ID);
            metodologiaa.Nombre = Nombre;
            metodologiaa.Descripcion = Descripcion;
            if (Opcion != "")
                metodologiaa.Formula = indicador + "/" + operacion + "/" + valor + "/" + Opcion;
            else
                metodologiaa.Formula = indicador + "/" + operacion + "/" + valor;
            //if (Inicial != null)
                metodologiaa.FechaInicial = Inicial;
            //if (Final != null)
                metodologiaa.FechaFinal = Final;

            //if (ModelState.IsValid)
          //  {
                db.Entry(metodologiaa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
           // }
           // return View(metodologia);
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