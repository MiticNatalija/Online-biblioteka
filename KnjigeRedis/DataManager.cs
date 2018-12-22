using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace KnjigeRedis
{
    public class DataManager
    {
      //  private string globalCounterKey = "next.url.id";

        readonly RedisClient redis = new RedisClient(Config.SingleHost);

        public DataManager()
        {
            Knjiga k = new Knjiga();
            k.ISBN = "123";
            k.Ime = "djsnvf";
            k.Autor = "rfdse";
            k.Datum = new DateTime(2000, 12, 12);

            if (redis.Exists("123")==1)
                return;
            redis.Set<string>("123", k.ToJsonString());
          //  redis.AddItemToSet(":knjige", "123");
            redis.PushItemToList("22" + ":knjige", k.ISBN);
            
        }
        public Knjiga getKnjiga(string isbn)
        {
           string knjiga = redis.Get<string>(isbn);
            //if (knjiga == null)
            //    return null;
            Knjiga k = (Knjiga)JsonSerializer.DeserializeFromString(knjiga, typeof(Knjiga));
            return k;
            

        }
        public void putKnjiga(Knjiga k)
        {
            if (redis.Exists(k.ISBN) == 1)
                return;
            redis.Set<string>(k.ISBN, k.ToJsonString());
            //redis.AddItemToSet(":knjige", k.ISBN);
            redis.PushItemToList("22"+":knjige", k.ISBN);
        }
        public List<Knjiga> getKnjige()
        {
            List<Knjiga> lista = new List<Knjiga>();
            long duzina=redis.LLen("22" + ":knjige");
            if (duzina > 10)
                duzina = 10;
            
            foreach (string id in redis.GetRangeFromList("22"+":knjige", 0, (int)duzina))
            {
                string knjiga= redis.Get<string>(id);
                Knjiga k = (Knjiga)JsonSerializer.DeserializeFromString(knjiga, typeof(Knjiga));
                lista.Add(k);
            }

            return lista;
        }
        public void deleteLista()
        {
            List<string> l = redis.GetAllItemsFromList("22" + ":knjige");
            foreach (string s in l)
                deleteKnjiga(s);
            redis.RemoveAllFromList("22" + ":knjige");
        }
        public void deleteKnjiga(string isbn)
        {
            redis.RemoveItemFromList("22" + ":knjige", isbn);
            var s=redis.SearchKeys(isbn + "*");
            foreach(string key in s)
            {
                redis.Remove(key);
            }
          //  redis.Remove(isbn);
           
        }
        public long getClicks(string isbn)
        {
            if (redis.Exists(isbn + ":clicks")==0)
            
                return 0;
            
            return redis.Get<long>(isbn+":clicks");
        }
        public void click(string isbn)
        {
            redis.Incr(isbn + ":clicks");
        }
       
        public void oceni(string isbn,int ocena)
        {
            if (redis.Exists(isbn + ":ocena") == 1)
            {
               string oc= redis.Get<string>(isbn + ":ocena");
                Ocena o = (Ocena)JsonSerializer.DeserializeFromString(oc, typeof(Ocena));
                o.brojGlasova++;
                if (ocena == 1)
                    o.jedan++;
                else if (ocena == 2)
                    o.dva++;
                else if (ocena == 3)
                    o.tri++;
                else if (ocena == 4)
                    o.cetiri++;
                else if (ocena == 5)
                    o.pet++;
                double pom = o.jedan + 2.0 * o.dva + 3.0 * o.tri + 4.0 * o.cetiri + 5.0 * o.pet;
                o.ocena = pom / o.brojGlasova;
                redis.Set<string>(isbn + ":ocena", o.ToJsonString());
            }
            else
            {
                Ocena o = new Ocena
                {
                    brojGlasova = 1,
                    ISBN = isbn,
                    jedan = 0,
                    dva = 0,
                    tri = 0,
                    cetiri = 0,
                    pet = 0,
                    ocena = ocena
       
                };
                if (ocena == 1)
                    o.jedan++;
                else if (ocena == 2)
                    o.dva++;
                else if (ocena == 3)
                    o.tri++;
                else if (ocena == 4)
                    o.cetiri++;
                else if (ocena == 5)
                    o.pet++;
                redis.Set<string>(isbn + ":ocena", o.ToJsonString());

            }

        }
        public double getOcena(string isbn)
        {
            if(redis.Exists(isbn+":ocena")==0)
            {
                return 0;
            }
            string o = redis.Get<string>(isbn + ":ocena");
            Ocena ocena = (Ocena)JsonSerializer.DeserializeFromString(o, typeof(Ocena));
           return System.Math.Round(ocena.ocena, 2);
            
        }
        public bool komentarisi(string isbn,string komentar,string user)
        {
            //mozda 1 2 3 da vraca jer samo jednom moze da se komentarise
            //ili da se ubaci brojac pa da moze vise puta da komentarise
            if (redis.Exists(isbn) == 0)
                return false;
            
            if (redis.Exists(isbn + user + ":koment") == 0)
            {
                redis.PushItemToList(isbn + ":komentari", komentar);
                redis.SetEntryInHash(isbn + user + ":koment", "komentar", komentar);
                redis.SetEntryInHash(isbn + user + ":koment", "likes", "0");
                redis.SetEntryInHash(isbn + user + ":koment", "unlikes", "0");
            }
            return true;

        }
        public List<string> getKomentarKnjiga(string isbn)
        {
            List<string> lista=new List<string>();
            long len = redis.LLen(isbn + ":komentari");
            if (len > 10)
                len = 10;
            foreach (string s in redis.GetRangeFromList(isbn + ":komentari", 0,(int) len))
            {
                lista.Add(s);
            }
            return lista;

        }
        public void like(string isbn,string user)
        {
            var hash = redis.GetAllEntriesFromHash(isbn + user + ":koment");
            int l =int.Parse(hash["likes"]);
            l++;
            redis.SetEntryInHash(isbn + user + ":koment", "likes", l.ToString());


        }
        public void unlike(string isbn, string user)
        {
            var hash = redis.GetAllEntriesFromHash(isbn + user + ":koment");
            int l = int.Parse(hash["unlikes"]);
            l++;
            redis.SetEntryInHash(isbn + user + ":koment", "unlikes", l.ToString());


        }
        public bool register(string user,string pass)
        {
            if (redis.Exists("user:" + user) == 1)
                return false;
            redis.Set<string>("user:"+user, pass);
            return true;
        }
        public bool login(string user,string pass)
        {
            string s = "";
             s = redis.Get<string>("user:" + user);
            if (s==null)
                return false;
            if (s.Equals(pass))
                return true;
            return false;
        }
        public bool admin(string user,string pass)
        {
            if (!user.Equals("admin"))
                return false;
            string s= redis.Get<string>("admin");
            if (pass.Equals(s))
                return true;
            return false;
        }

    }
}
