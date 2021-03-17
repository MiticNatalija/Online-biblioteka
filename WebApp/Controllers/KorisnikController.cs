using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KnjigeRedis;
using System.Web.SessionState;


namespace WebApp.Controllers
{
  //  [SessionTimeout]
    public class KorisnikController : Controller
    {
        DataManager manager = new DataManager();
        // GET: Korisnik
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult IndexLog()
        {
            int g = 0;
            return View("Logovanje",g);
        }
        public ActionResult IndexReg()
        {
            int g = 0;
            return View("Registracija",g);
        }
        
        public ActionResult Logovanje(FormCollection collection)
        {
            bool pom = manager.login(Request.Form["username"], Request.Form["password"]);

            int g = 1;
            if (!pom)
                return View("Logovanje", g);
            Session["korisnik"] = Request.Form["username"];

              Session.Timeout = 3;
            //  var timeOut = 60;
            //  return RedirectToAction("Index", "Home",timeOut);
           // var timeExpire = DateTime.UtcNow.AddSeconds(65);
            
            return RedirectToAction("Index", "Home");


        }
      

        public ActionResult Registrovanje(FormCollection collection)
        {
            bool pom = manager.register(Request.Form["username"], Request.Form["password"]);
            int g = 1;
            if(!pom)
            return View("Registracija", g);
            return IndexLog();
        }

        public ActionResult LogOut(FormCollection collection)
        {
           
            manager.deleteKorpa(Session["korisnik"].ToString());
            Session["korisnik"] = null;
            return RedirectToAction("Index", "Home");

        }
        public ActionResult Nazad(FormCollection collection)
        {

            return RedirectToAction("Index", "Home");
        }
    }
}