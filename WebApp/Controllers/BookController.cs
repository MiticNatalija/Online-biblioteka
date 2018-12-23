using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Books;

namespace WebApp.Controllers
{
    public class BookController : Controller
    {
        DataManager manager = new DataManager();
        // GET: Book
        public ActionResult Index(string isbn)
        {
            List<List<string>> l = new List<List<string>>();
            Knjiga k = manager.getKnjiga("666");
            
            double o = manager.getOcena(isbn);
            List<string> komentari = manager.getKomentarKnjiga(isbn);
           

            List<string> knjiga = new List<string>();
            knjiga.Add(k.ISBN);
            knjiga.Add(k.Ime);
            knjiga.Add(k.Autor);
            knjiga.Add(k.Datum.ToShortDateString());
            knjiga.Add(o.ToString());
            l.Add(knjiga);

            l.Add(komentari);



            return View(l);
        }
    }
}