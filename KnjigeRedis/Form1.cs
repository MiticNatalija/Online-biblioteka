using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnjigeRedis
{
    public partial class Form1 : Form
    {
        DataManager manager;
        public Form1()
        {
            InitializeComponent();
           manager = new DataManager();
           manager.deleteLista();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            Knjiga k = manager.getKnjiga("123");
            if(k!=null)
           MessageBox.Show( k.Ime);
           

             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Knjiga k = new Knjiga
            {
                Autor = "Ivo Andric",
                Datum = new DateTime(1990, 10, 10),
                Ime = "Na Drini cuprija",
                ISBN = "123"
            };
          

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Knjiga> lista = manager.getKnjige(10);
            foreach(Knjiga k in lista)
            {
                if (k == null)
                    break;
                MessageBox.Show(k.ISBN + " " + k.Ime);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
          //  manager.deleteKnjiga("666");
          //  manager.deleteKnjiga("123");
            manager.deleteLista();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(manager.getClicks("123").ToString());
            manager.click("123");
            MessageBox.Show(manager.getClicks("123").ToString());

        }

        private void button5_Click(object sender, EventArgs e)
        {
            double ocena = manager.getOcena("123");
            MessageBox.Show(ocena.ToString());
            int oc =int.Parse(txtOcena.Text);
            manager.oceni("123", oc,"natica");
         //   manager.oceni("123", 4);
         //   ocena=manager.getOcena("123");
            MessageBox.Show(ocena.ToString());


        }

        private void button6_Click(object sender, EventArgs e)
        {
            //manager.deleteKomentar("123", "ja");
            //manager.deleteKomentar("123", "dd");
            //manager.deleteKomentar("123", "samo");
            //manager.deleteKomentar("123", "ok");

            //MessageBox.Show(manager.komentarisi("123", "lepa knjiga", "ja").ToString());
            //MessageBox.Show(manager.komentarisi("123", "Meni se uopste ne svidja.", "dd").ToString());
            //MessageBox.Show(manager.komentarisi("123", "Meni se uopste ne svidja.", "samo").ToString());
            //MessageBox.Show(manager.komentarisi("123", "Meni se uopste ne svidja.", "ok").ToString());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<List<string>> l = manager.getKomentarKnjiga("123",2);
            foreach(List<string> kom in l)
            {
                foreach(string s in kom)
                MessageBox.Show(s);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
           MessageBox.Show( manager.register("nat", "sifra").ToString());
            MessageBox.Show(manager.register("naa", "sifra").ToString());

        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show(manager.login("nat", "sifra").ToString());
            MessageBox.Show(manager.login("nata", "sifra").ToString());
            MessageBox.Show(manager.login("nat", "sifa").ToString());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            manager.addKorpa("111", "neki");
            manager.addKorpa("155", "neki");
            manager.addKorpa("111", "neki");

            string[] s = manager.getKorpa("neki");
            foreach(string si in s)
            {
                MessageBox.Show(si);
            }
            manager.deleteFromKorpa("neki", "111");
            s = manager.getKorpa("neki");
            foreach (string si in s)
            {
                MessageBox.Show(si);
            }
            s = manager.getKorpa("neki");
            manager.deleteKorpa("neki");
            s = manager.getKorpa("neki");
            foreach (string si in s)
            {
                MessageBox.Show(si);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            manager.deleteAll();
           // manager.isporuci("natica");
            //manager.isporuci("nata");
            //List<List<string>> l = manager.getNarudzbine();

            //foreach (List<string> ls in l)
            //    foreach (string s in ls)
            //    {
            //        MessageBox.Show(s);
            //    }
        }
    }
}
