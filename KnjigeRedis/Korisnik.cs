using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace KnjigeRedis
{
   public class Korisnik
    {
        public string UserName { get; set; }
        public string Ime{ get; set; }
        public string Prezime { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }


        public string ToJsonString()
        {
            return JsonSerializer.SerializeToString<Korisnik>(this);
        }
    }

}
