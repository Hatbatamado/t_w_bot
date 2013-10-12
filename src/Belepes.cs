using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace bot_v4
{
    public partial class Belepes : Form
    {
        public Belepes()
        {
            InitializeComponent();
            //------------------------Set the form elements for fill
            Alap();
        }
        TextBox felha, jelsz;
        private void Alap()
        {
            //------------------------Set the form elements for fill
            string fel = "";
            string jel = "";
            //------------------------
            if (File.Exists("belep.txt"))
            {
                StreamReader sr = new StreamReader("belep.txt");
                fel = sr.ReadLine();
                jel = sr.ReadLine();
                sr.Close();
            }
            //------------------------Label -> Textbox
            Label felh = new Label();
            Controls.Add(felh);
            felh.Text = "Felhasználónév:";
            felh.Location = new Point(20, 20);
            felh.Size = new System.Drawing.Size(90, 20);

            felha = new TextBox();
            Controls.Add(felha);
            felha.Location = new Point(110, 18);
            felha.Text = fel;
            //------------------------Label -> Textbox
            Label jelszo = new Label();
            Controls.Add(jelszo);
            jelszo.Text = "Jelszó:";
            jelszo.Location = new Point(68, 50);
            jelszo.Size = new System.Drawing.Size(40, 20);

            jelsz = new TextBox();
            Controls.Add(jelsz);
            jelsz.Location = new Point(110, 48);
            jelsz.Text = jel;
            //------------------------
            Button ok = new Button();
            Controls.Add(ok);
            ok.Text = "OK";
            ok.Location = new Point(75, 80);
            ok.Click += new EventHandler(ok_Click);
                        
        }

        void ok_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("belep.txt", false);
            sw.WriteLine(felha.Text);
            sw.WriteLine(jelsz.Text);
            sw.Close();

            Form1 back = new Form1();
            this.Hide();
            back.Show();
        }

        private void Belepes_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
