using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Books;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        DataManager manager = new DataManager();
        // GET: Home
        public ActionResult Index()
        {
            List<Knjiga> lista = manager.getKnjige();
            List<List<string>> k = new List<List<string>>();
            foreach (Knjiga kn in lista)
            {
                List<string> l=new List<string>();
                l.Add(kn.ISBN);
                l.Add( kn.Ime);
                l.Add(kn.Autor);
                l.Add(kn.Datum.ToShortDateString());

                k.Add(l);
             
            }
            
            return View(k);
        }
    }
}