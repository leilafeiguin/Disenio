using Diseño.DAL;
using Diseño.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diseño.Controllers
{
    public class LoginController : Controller
    {
        private DondeInviertoContext db = new DondeInviertoContext();
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View("Index", "_LayoutLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario objUser)
        {
            if (ModelState.IsValid)
            {    
                var obj = db.Usuarios.Where(a => a.Username.Equals(objUser.Username) && a.Password.Equals(objUser.Password)).FirstOrDefault();
                if (obj != null)
                {
                    Session["UserID"] = obj.ID.ToString();
                    Session["Username"] = obj.Username.ToString();
                    return View("../Home/Index");
                }
                
            }
            TempData["msgUsuarioInvalido"] = "<script>alert('El usuario o contraseña ingresados son incorrectos');</script>";
            return RedirectToAction("Index"); 
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["Username"] = null;
            return RedirectToAction("Index"); 
        }
	}
}