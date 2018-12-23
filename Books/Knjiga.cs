using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace Books
{
    public class Knjiga
    {
        public string ISBN { get; set; }
        public string Ime { get; set; }
        public string Autor { get; set; }
        public DateTime Datum { get; set; }


        public string ToJsonString()
        {
            return JsonSerializer.SerializeToString<Knjiga>(this);
        }
    }
}
