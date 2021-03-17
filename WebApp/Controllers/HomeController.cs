using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KnjigeRedis;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        DataManager manager = new DataManager();
        List<Knjiga> lista;
        int brojKnjiga = 10;
        // GET: Home
        public ActionResult Index()
        {
            if(lista==null)
             lista = manager.getKnjige(brojKnjiga);
            List<List<string>> k = new List<List<string>>();
            foreach (Knjiga kn in lista)
            {
                if (kn == null)
                    continue;
                List<string> l=new List<string>();
                l.Add(kn.ISBN);
                l.Add( kn.Ime);
                l.Add(kn.Autor);
                l.Add(kn.Datum.ToShortDateString());

                k.Add(l);
             
            }
           // ViewBag.timeExpire = DateTime.UtcNow.AddSeconds(65);
            return View("Home",k);
        }
        public ActionResult UcitajKnjige(FormCollection collection)
        {
            brojKnjiga += 10;
            return Index();
        }
    }
}