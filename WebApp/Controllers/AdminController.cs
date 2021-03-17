using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KnjigeRedis;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        DataManager manager = new DataManager();
        // GET: Admin
        public ActionResult Index()
        {
            int b = 0;
            return View(b);
        }
        public ActionResult IndexAdd()
        {
            return View("DodajKnjiguView");
        }
        public ActionResult IndexDel()
        {
            return View("ObrisiKnjiguView");
        }
        public ActionResult IndexNarudzbina()
        {
          List<List<string>> l= manager.getNarudzbine();

            return View("NarudzbinaView",l);
        }
        public ActionResult Isporuci(FormCollection collection)
        {

            manager.isporuci(Request.Form["kupac"]);
            return RedirectToAction("IndexNarudzbina");
        }

        public ActionResult AddKnjiga(FormCollection collection)
        {

            Knjiga k = new Knjiga();
            k.ISBN = Request.Form["isbn"];
            k.Ime = Request.Form["ime"];
            k.Autor = Request.Form["autor"];
            k.Datum =DateTime.Parse(Request.Form["datum"]);
            manager.putKnjiga(k);
            int s = 1;
            return View("DodajKnjiguView",s);
        }
        public ActionResult DeleteKnjiga(FormCollection collection)
        {
            manager.deleteKnjiga(Request.Form["isbn"]);
            int s = 1;
            return View("ObrisiKnjiguView",s);
        }
        public ActionResult LogAdmin(FormCollection collection)
        {
            bool pom = manager.admin(Request.Form["username"], Request.Form["password"]);
            if (pom)
                return View("DodajKnjiguView");
            int b = 1;
            return View("Index",b);
        }
      

    }
}