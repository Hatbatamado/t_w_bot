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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Button belep = new Button();
            Button bot = new Button();
            Init(belep, bot);
        }

        private void Init(Button belep, Button bot)
        {
            //------------------------Button: user details
            Controls.Add(belep);
            belep.Text = "Belépési adatok";
            belep.Location = new Point(90, 50);
            belep.Size = new Size(100, 23);
            belep.Click += new EventHandler(belep_Click);
            //------------------------

            //------------------------Button: run bot
            Controls.Add(bot);
            bot.Text = "Böngésző indítása";
            bot.Location = new Point(85, 95);
            bot.Size = new Size(110, 23);
            bot.Click += new EventHandler(bot_Click);
            //------------------------
        }

        void bot_Click(object sender, EventArgs e)
        {
            //------------------------Run bot
            string seged = Belephet();
            if (seged != "")
            {
                Bongeszo gui = new Bongeszo(seged);
                this.Hide();
                gui.Show();
            }
        }

        private string Belephet()
        {
            //------------------------Login data check
            string vissza = "";
            if (File.Exists("belep.txt"))
            {
                StreamReader sr = new StreamReader("belep.txt");
                string felh = sr.ReadLine();
                string jel = sr.ReadLine();
                sr.Close();

                if (felh == null || felh == "" || felh == " ")
                    MessageBox.Show("A belépési adatok hiányzoknak");
                else
                    vissza = felh + " " + jel;
            }
            else
                MessageBox.Show("A belépési adatok hiányzoknak");
            return vissza;
        }

        void belep_Click(object sender, EventArgs e)
        {
            //------------------------User details
            Belepes be = new Belepes();
            this.Hide();
            be.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
