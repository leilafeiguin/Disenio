//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Diseño;
//using Diseño.Controllers;
//using Diseño.Models;

//namespace UnitTestProject2.Controllers
//{
//    [TestClass]
//    class EmpresasControllerTest
//    {
//        [TestMethod]
//        public void EsValida_EmpresaSinNombre()
//        {
//            EmpresasController controller = new EmpresasController();

//            var empresa = new Empresa() { Nombre = string.Empty };

//            var resultado = controller.EsValidaEmpresa(empresa);

//            Assert.IsFalse(resultado);
//        }
//    }
//}
