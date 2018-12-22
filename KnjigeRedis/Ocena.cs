using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace KnjigeRedis
{
   public class Ocena
    {
        public string ISBN { get; set; }
        public int brojGlasova { get; set; }
        //ili ukupan zbir
        public int pet { get; set; }
        public int cetiri { get; set; }
        public int tri { get; set; }
        public int dva { get; set; }
        public int jedan { get; set; }
        public double ocena { get; set; }

        public string ToJsonString()
        {
            return JsonSerializer.SerializeToString<Ocena>(this);
        }
    }
}
