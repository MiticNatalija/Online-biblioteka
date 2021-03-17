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
       
        readonly RedisClient redis = new RedisClient(Config.SingleHost);
        private string globalCounterKey = "next.comment.id";
        public DataManager()
        {
            var test = redis.Get<object>(globalCounterKey);
            if (test==null)
            {
                var redisCounterSetup = redis.As<string>();
                redisCounterSetup.SetEntry(globalCounterKey, "1");
            }

            Knjiga k = new Knjiga();
            k.ISBN = "1001";
            k.Ime = "Majstor i Margarita";
            k.Autor = "Mihail Bulgakov";
            k.Datum = new DateTime(1966, 12, 12);

            if (redis.Exists("1001") == 0)
            {
                redis.Set<string>("1001", k.ToJsonString());
                redis.LPush("22" + ":knjige", k.ISBN.ToAsciiBytes());
            }
            if (redis.Exists("1002") == 0)
            {
                k = new Knjiga();
                k.ISBN = "1002";
                k.Ime = "Vreme vlasti";
                k.Autor = "Dobrica Cosic";
                k.Datum = new DateTime(1996, 12, 12);
                redis.Set<string>("1002", k.ToJsonString());
                redis.LPush("22" + ":knjige", k.ISBN.ToAsciiBytes());
            }
            if (redis.Exists("1003") == 0)
            {
                k = new Knjiga();
                k.ISBN = "1003";
                k.Ime = "Tvrdjava";
                k.Autor = "Mesa Selimovic";
                k.Datum = new DateTime(1991, 12, 12);
                redis.Set<string>("1003", k.ToJsonString());
                redis.LPush("22" + ":knjige", k.ISBN.ToAsciiBytes());
            }
          
            if (redis.Exists("admin") == 0)
                redis.Set<string>("admin", "admin");

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
            redis.LPush("22"+":knjige", k.ISBN.ToAsciiBytes());
        }
        public List<Knjiga> getKnjige(int broj)
        {
            List<Knjiga> lista = new List<Knjiga>();
            long duzina=redis.LLen("22" + ":knjige");
            if (duzina > broj)
                duzina = broj-1;
            
            foreach (string id in redis.GetRangeFromList("22"+":knjige", 0, (int)duzina))
            {
                string knjiga= redis.Get<string>(id);
                Knjiga k = (Knjiga)JsonSerializer.DeserializeFromString(knjiga, typeof(Knjiga));
                lista.Insert(0,k);
            }
            lista.Reverse();
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
       
        public void oceni(string isbn,int ocena,string user)
        {
            if (redis.SIsMember(isbn + ":oceneuser", user.ToAsciiBytes()) == 1)
            {
              string pom=  redis.Get<string>(isbn + user + ":ocena");
                redis.Set<string>(isbn + user + ":ocena", ocena.ToString());
                int p;
                int.TryParse(pom, out p);
               

                string oc = redis.Get<string>(isbn + ":ocena");
                Ocena o = (Ocena)JsonSerializer.DeserializeFromString(oc, typeof(Ocena));
                if (p == 1)
                    o.jedan--;
                else if (p == 2)
                    o.dva--;
                else if (p == 3)
                    o.tri--;
                else if (p == 4)
                    o.cetiri--;
                else if (p == 5)
                    o.pet--;
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
                double pomm = o.jedan + 2.0 * o.dva + 3.0 * o.tri + 4.0 * o.cetiri + 5.0 * o.pet;
                o.ocena = pomm / o.brojGlasova;
                redis.Set<string>(isbn + ":ocena", o.ToJsonString());

                return;
            }
            redis.Set<string>(isbn + user + ":ocena", ocena.ToString());
            redis.AddItemToSet(isbn + ":oceneuser", user);
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
        public string komentarisi(string isbn,string komentar,string user)
        {
         
            if (redis.Exists(isbn) == 0 || komentar=="")
                 return "";
            long nextId = redis.Incr(globalCounterKey);
            string g = isbn + user + nextId.ToString("x");
            redis.LPush(isbn + ":komentari", g.ToAsciiBytes());
            redis.SetEntryInHash(g + ":koment", "user", user);
            redis.SetEntryInHash(g + ":koment", "komentar", komentar);
            redis.SetEntryInHash(g + ":koment", "likes", "0");
            redis.SetEntryInHash(g + ":koment", "unlikes", "0");

            //if (redis.Exists(isbn + user + ":koment") == 0)
            //{

            //    string g = isbn + user;
            //    redis.LPush(isbn + ":komentari", g.ToAsciiBytes());
            //    redis.SetEntryInHash(isbn + user + ":koment", "user", user);
            //    redis.SetEntryInHash(isbn + user + ":koment", "komentar", komentar);
            //    redis.SetEntryInHash(isbn + user + ":koment", "likes", "0");
            //    redis.SetEntryInHash(isbn + user + ":koment", "unlikes", "0");
            //}
            return g;
         
        }
      
        public List<List<string>> getKomentarKnjiga(string isbn,int br)
        {
          

            List<List<string>> lista=new List<List<string>>();
            long ukupno = redis.LLen(isbn + ":komentari");
            if (ukupno > br)
                ukupno = br-1;
            List<string> komen = redis.GetRangeFromList(isbn + ":komentari", 0, (int)ukupno);
           
            foreach (string s in komen)
            {
                var hash = redis.GetAllEntriesFromHash(s+":koment");
                List<string> pom = new List<string>(); //ovde da se ubaci jos jedno polje
               
                pom.Add(hash["user"]);
                pom.Add(hash["komentar"]);
                pom.Add(hash["likes"]);
                pom.Add(hash["unlikes"]);
                pom.Add(s);
                lista.Add(pom);
            }
            lista.Reverse();
            return lista;

        }
        public void deleteKomentar(string isbn,string user)
        {
           // redis.Remove(isbn + ":komentari");
           // redis.Remove(isbn + user + ":koment");
        }
      
       

        public void like(string isbn,string owner,string user,string komentId)
        {
            //owner je g
            if (owner.Equals(user))
                return;
            if (redis.SIsMember(komentId + ":likes", user.ToAsciiBytes()) == 1)
            {
                return;
            }
            if (redis.SIsMember(komentId + ":unlikes", user.ToAsciiBytes()) == 1)
            {

                redis.RemoveItemFromSet(komentId + ":unlikes", user);
                // var h = redis.GetAllEntriesFromHash(isbn + owner + ":koment");
                var h = redis.GetAllEntriesFromHash(komentId + ":koment");
                int li = int.Parse(h["unlikes"]);
                if (li > 0)
                {
                    li--;
                    redis.SetEntryInHash(komentId + ":koment", "unlikes", li.ToString());
                }
            }

            redis.AddItemToSet(komentId + ":likes", user);
            var hash = redis.GetAllEntriesFromHash(komentId + ":koment");
            int l =int.Parse(hash["likes"]);
            l++;
            redis.SetEntryInHash(komentId + ":koment", "likes", l.ToString());


        }
        
        public void unlike(string isbn, string owner,string user,string komentId)//treba da se doradi ako radi za lajk
        {
            if (owner.Equals(user))
                return;
            if ( redis.SIsMember(komentId + ":unlikes", user.ToAsciiBytes())==1)
            {
                return;
            }
            if (redis.SIsMember(komentId + ":likes", user.ToAsciiBytes()) == 1)
            {

                redis.RemoveItemFromSet(komentId + ":likes", user);
                var h = redis.GetAllEntriesFromHash(komentId + ":koment");
                int li = int.Parse(h["likes"]);
                if (li > 0)
                {
                    li--;
                    redis.SetEntryInHash(komentId + ":koment", "likes", li.ToString());
                }
            }
            redis.AddItemToSet(komentId + ":unlikes", user);
            var hash = redis.GetAllEntriesFromHash(komentId + ":koment");
            int l = int.Parse(hash["unlikes"]);
            l++;
            redis.SetEntryInHash(komentId + ":koment", "unlikes", l.ToString());


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
        public void addKorpa(string isbn, string user)
        {
           redis.AddItemToSet(user + ":korpa", isbn);
            redis.Expire(user + ":korpa", 100);  // posle nekog vremena da se izbrise korpa
        }
        public string[] getKorpa(string user)
        {
            string[] korpa;
            korpa=  redis.GetAllItemsFromSet(user + ":korpa").ToArray<string>();

            return korpa;
        }
        public void deleteFromKorpa(string user,string isbn)
        {
            redis.RemoveItemFromSet(user + ":korpa", isbn);
        }
        public void deleteKorpa(string user)
        {
            redis.Remove(user + ":korpa");
        }
        public void naruci(Korisnik kor)
        {
            string[] knjige = getKorpa(kor.UserName);
            if(redis.Exists(kor.UserName+":narucilac")==0)
            redis.Set<string>(kor.UserName + ":narucilac", kor.ToJsonString());
            foreach (string s in knjige)
                redis.AddItemToSet(kor.UserName + ":narudzbina",s);

            deleteKorpa(kor.UserName);
        }
        public List<List<string>> getNarudzbine()
        {
           List<string> lista= redis.SearchKeys("*" + ":narucilac");
            List<List<string>> toret = new List<List<string>>();
            List<Korisnik> korisnici = new List<Korisnik>();
            foreach (string s in lista)
            {
                string val = redis.Get<string>(s);
                Korisnik k = (Korisnik)JsonSerializer.DeserializeFromString(val, typeof(Korisnik));
                List<string> pom = new List<string>();
                pom.Add(k.UserName);
                pom.Add(k.Ime + " " + k.Prezime);
                pom.Add(k.Adresa);
                pom.Add(k.Telefon);
                string[] knjige = redis.GetAllItemsFromSet(k.UserName + ":narudzbina").ToArray<string>();
                foreach(string kn in knjige)
                {
                    pom.Add(kn);
                }
                toret.Add(pom);
            }
            return toret;

        }
        public void isporuci(string user)
        {
            redis.Remove(user + ":narudzbina");
            redis.Remove(user + ":narucilac");
        }
        public void deleteAll()
        {
            List<string> l = redis.SearchKeys("*");
            foreach (string key in l)
                redis.Remove(key);
        }

    }
}
