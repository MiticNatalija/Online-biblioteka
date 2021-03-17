using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KnjigeRedis;

namespace WebApp.Controllers
{
    public class BookController : Controller
    {
        DataManager manager = new DataManager();
        bool first = true;
        int komentariBroj = 2;
        // GET: Book
        public ActionResult Index()
        {
          //  List<List<string>> l = new List<List<string>>();
            List<object> l = new List<object>();
            Knjiga k = manager.getKnjiga("666");
            
            double o = manager.getOcena("666");
            List<List<string>> komentari = manager.getKomentarKnjiga("666",2);
           

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
        public ActionResult GetKnjiga(FormCollection collection)
        {
     

            Knjiga k = manager.getKnjiga(Request.Form["isbn"]);
            if (k == null)
                return View("Index", null);
            if(first)
            manager.click(Request.Form["isbn"]);
          //  first = true;
            long cl=  manager.getClicks(Request.Form["isbn"]);
                double o = manager.getOcena(Request.Form["isbn"]);

            ViewBag.isbn = k.ISBN;
            ViewBag.ime = k.Ime;
            ViewBag.autor = k.Autor;
            ViewBag.datum = k.Datum.ToShortDateString();
            if (o == 0)
                ViewBag.ocena = "Nema ocene.";
            else
            ViewBag.ocena = o.ToString();
            ViewBag.pregledi = cl.ToString();

          
            List<List<string>> komentari = manager.getKomentarKnjiga(Request.Form["isbn"],komentariBroj);
       



                return View("Index",komentari);
            
        }
        public ActionResult GetKnjigaLink(string isbn)
        {


            Knjiga k = manager.getKnjiga(isbn);
            if (k == null)
                return View("Index", null);
            if(first)
            manager.click(isbn);
            //first = true;
            long cl = manager.getClicks(isbn);
            double o = manager.getOcena(isbn);
            List<List<string>> komentari = manager.getKomentarKnjiga(isbn,komentariBroj);
            ViewBag.isbn = k.ISBN;
            ViewBag.ime = k.Ime;
            ViewBag.autor = k.Autor;
            ViewBag.datum = k.Datum.ToShortDateString();
            
            if (o == 0)
                ViewBag.ocena = "Nema ocene.";
            else
            ViewBag.ocena = o.ToString();
            ViewBag.pregledi = cl.ToString();


            return View("Index", komentari);

        }
        public ActionResult PutKomentar(FormCollection collection)
        {
           string id= manager.komentarisi(Request.Form["isbn"], Request.Form["komentar"], Session["korisnik"].ToString());
            first = false;
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult Like(FormCollection collection)
        {
            //manager.like(Request.Form["isbn"], Request.Form["user"],Session["korisnik"].ToString());
            string a = Request.Form["isbn"];
            string b= Request.Form["komentarId"];
            string c = Session["korisnik"].ToString();
            manager.like(Request.Form["isbn"], Request.Form["user"], Session["korisnik"].ToString(), Request.Form["komentarId"]);
            first = false;
            
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult Unlike(FormCollection collection)
        {
            manager.unlike(Request.Form["isbn"], Request.Form["user"],Session["korisnik"].ToString(), Request.Form["komentarId"]);
            first = false;
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult Nazad(FormCollection collection)
        {
            first = true;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult PutOcena(FormCollection collection)
        {
            manager.oceni(Request.Form["isbn"], int.Parse(Request.Form["ocenaradio"]),Session["korisnik"].ToString());
            first = false;
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult UcitajKomentare(FormCollection collection)
        {
            komentariBroj += 5;
            first = false;
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult AddKorpa(FormCollection collection)
        {
            first = false;
            if (Session["korisnik"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            manager.addKorpa(Request.Form["isbn"], Session["korisnik"].ToString());
            TempData["dodato"] = "dodato";
            return GetKnjigaLink(Request.Form["isbn"]);
        }
        public ActionResult KorpaPrikaz(FormCollection collection)
        {
            if (Session["korisnik"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string[] sifre = manager.getKorpa(Session["korisnik"].ToString());
            if (sifre.Length > 0)
            {
                List<string> li = new List<string>();
                foreach(string s in sifre)
                {
                    Knjiga k = manager.getKnjiga(s);
                    string pom =  k.ISBN +" " + k.Ime + " " + k.Autor;
                    li.Add(pom);
                }
               
                return View("Korpa", li);
            }
            return View("Korpa",null);
        }
        public ActionResult RemoveKorpa(FormCollection collection)
        {
            if (Session["korisnik"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            manager.deleteFromKorpa(Session["korisnik"].ToString(), Request.Form["isbn"]);
            return RedirectToAction("KorpaPrikaz");
        }
        public ActionResult Naruci(FormCollection collection)
        {
            if (Session["korisnik"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Korisnik kor = new Korisnik
            {
                UserName=Session["korisnik"].ToString(),
                Ime=Request.Form["ime"],
                Prezime=Request.Form["prezime"],
                Adresa=Request.Form["adresa"],
                Telefon=Request.Form["telefon"]
            };
            manager.naruci(kor);
            TempData["naruceno"] = "naruceno";
            return RedirectToAction("KorpaPrikaz");
        }

    }
}