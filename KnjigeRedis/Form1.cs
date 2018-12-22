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
         //   manager.deleteLista();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            Knjiga k = manager.getKnjiga("123");
            if(k!=null)
           MessageBox.Show( k.Ime);
            //mora prvo da se doda ova knjiga

             k = manager.getKnjiga("666");
            if (k != null)
                MessageBox.Show(k.Ime);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Knjiga k = new Knjiga
            {
                Autor = "Dragan",
                Datum = new DateTime(1990, 10, 10),
                Ime = "Kako da se utepas",
                ISBN = "666"
            };
            manager.putKnjiga(k);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Knjiga> lista = manager.getKnjige();
            foreach(Knjiga k in lista)
            {
                if (k == null)
                    break;
                MessageBox.Show(k.ISBN + " " + k.Ime);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            manager.deleteKnjiga("666");
            manager.deleteKnjiga("123");
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
            manager.oceni("123", oc);
         //   manager.oceni("123", 4);
         //   ocena=manager.getOcena("123");
            MessageBox.Show(ocena.ToString());


        }

        private void button6_Click(object sender, EventArgs e)
        {
          MessageBox.Show(manager.komentarisi("666", "lepa knjiga", "ja").ToString());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<string> l = manager.getKomentarKnjiga("666");
            foreach(string kom in l)
            {
                MessageBox.Show(kom);
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
    }
}
