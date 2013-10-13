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
    public partial class Bongeszo : Form
    {
        private static Random R = new Random();
        private static WebBrowser web;
        private string data; //user login data
        private static bool botvedelem = false; //bot protection
        private static bool mehet = true; //enables/disables the specific bot apps to run
        private static Falvak[] falvak; //villages data: ID, name, koord
        //------------------------GUI vars
        private static Label url = new Label(); //url label
        private static Button betoltve = new Button(); //villages data load button
        private static Button bbb = new Button(); //navigate left
        private static Button jjj = new Button(); //navigate right
        //------------------------

        public Bongeszo(string data)
        {
            InitializeComponent();
            //------------------------Window max height, half width
            this.Size = new System.Drawing.Size(Screen.GetWorkingArea(this).Width / 2, Screen.GetWorkingArea(this).Height);
            this.Location = new Point(0, 0);
            //------------------------
            this.data = data;
            Timer_init();
            Belep();
        }

        private static Timer[] belep = new Timer[6];
        private static Timer pirosgomb = new Timer();
        private void Timer_init()
        {
            //------------------------Timers initalize
            #region belep timers
            for (int i = 0; i < belep.Length; i++)
                belep[i] = new Timer();
            //------------------------1) webbrowser setup
            belep[0].Interval = R.Next(250, 500);
            belep[0].Tick += new EventHandler(belep1_Tick);
            //------------------------2) website load for login info
            //for slower connection, so the program won't crash
            //document_completed could do this too, but for now this is enough
            //TODO: document_completed version
            belep[1].Interval = R.Next(1200, 1800);
            belep[1].Tick += new EventHandler(belep2_Tick);
            //------------------------3) login data fill
            belep[2].Interval = R.Next(250, 500);
            belep[2].Tick += new EventHandler(belep3_Tick);
            //------------------------4) choose world popup
            belep[3].Interval = R.Next(500, 750);
            belep[3].Tick += new EventHandler(belep4_Tick);
            //------------------------5) login
            belep[4].Interval = R.Next(1200, 1800);
            belep[4].Tick += new EventHandler(belep5_Tick);
            //------------------------6) navigate to villages list page
            //at midnight it show the stats if you didn't disabled it yet
            //so the program could crash while trying to get villages data
            belep[5].Interval = R.Next(2000, 2200);
            //slower connection + lot villages = this time needed atleast
            belep[5].Tick += new EventHandler(belep6_Tick);
            #endregion
            //------------------------
            pirosgomb.Interval = R.Next(500, 750);
            pirosgomb.Tick += new EventHandler(pirosgomb_Tick);
        }

        #region login
        private void Belep()
        {
            //------------------------Webbrowser settings
            web = new WebBrowser();
            web.ScriptErrorsSuppressed = true; //IE error hide
            Controls.Add(web);
            web.Dock = DockStyle.Bottom;
            //-85px from the top for place to labels, buttons, etc.
            web.Size = new Size(Screen.GetWorkingArea(this).Width / 2, Screen.GetWorkingArea(this).Height - 85);
            //------------------------Sleep 250-500 ms
            belep[0].Start(); //a bit wait, because it would crash without it
        }

        private void belep1_Tick(object sender, EventArgs e)
        {
            //------------------------Website login part
            belep[0].Stop();
            web.Navigate("http://www.klanhaboru.hu/index.php");
            //------------------------
            belep[1].Start();
        }

        private void belep2_Tick(object sender, EventArgs e)
        {
            //------------------------Fill in login data
            belep[1].Stop();
            Belepesi_adatok(data);
            //------------------------
            belep[2].Start();
        }

        private void Belepesi_adatok(string data)
        {
            //------------------------Login data
            string[] seged = data.Split(' ');
            //------------------------Fill in login data
            web.Document.GetElementById("user").SetAttribute("value", seged[0]);
            web.Document.GetElementById("password").SetAttribute("value", seged[1]);
        }

        private void belep3_Tick(object sender, EventArgs e)
        {
            //------------------------Click on login button
            belep[2].Stop();
            foreach (HtmlElement element in web.Document.GetElementsByTagName("SPAN"))
            {
                if (element.GetAttribute("innerHTML") == "Bejelentkezés")
                {
                    element.InvokeMember("click");
                    break;
                }
            }
            belep[3].Start();
        }

        private void belep4_Tick(object sender, EventArgs e)
        {
            //------------------------World choosing (v17)
            belep[3].Stop();
            foreach (HtmlElement element in web.Document.GetElementsByTagName("SPAN"))
            {
                if (element.GetAttribute("innerHTML") == "Világ 17")
                {
                    element.InvokeMember("click");
                    break;
                }
            }
            //------------------------
            //this is must have now, because of bot protection
            //we not wanna get banend ;)
            web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            //------------------------
            belep[4].Start();
        }

        void belep5_Tick(object sender, EventArgs e)
        {
            //------------------------Navigate to villages list page
            belep[4].Stop();
            web.Navigate("http://hu17.klanhaboru.hu/game.php?screen=overview_villages&intro");
            //------------------------
            belep[5].Start();
        }
        #endregion
        void belep6_Tick(object sender, EventArgs e)
        {
            //------------------------GUI setup
            belep[5].Stop();
            //------------------------bot protection check
            if (web.Document.GetElementById("bot_check_image") != null)
                botvedelem = true;
            //------------------------GUI
            else
            {
                //------------------------Current URL
                Controls.Add(url);
                url.Text = web.Url.ToString();
                url.Location = new Point(10, 8);
                url.Size = new System.Drawing.Size(355, 16);
                //------------------------Villages data load button
                Controls.Add(betoltve);
                betoltve.Text = "";
                betoltve.Location = new Point(375, 4);
                betoltve.Size = new Size(20, 20);
                betoltve.BackColor = Color.Red;
                betoltve.Click += new EventHandler(betoltve_Click); //data load/reload
                //------------------------Navigate left
                Controls.Add(bbb);
                bbb.Text = "<-";
                bbb.Size = new System.Drawing.Size(30, 22);
                bbb.Location = new Point(415, 4);
                bbb.Click += new EventHandler(bbb_Click);
                //------------------------Navigate right
                Controls.Add(jjj);
                jjj.Text = "->";
                jjj.Size = new System.Drawing.Size(30, 22);
                jjj.Location = new Point(455, 4);
                jjj.Click += new EventHandler(jjj_Click);

                pirosgomb.Start();
            }
        }

        #region betoltve
        void betoltve_Click(object sender, EventArgs e)
        {
            //------------------------Color check, always should run without any problem
            if (betoltve.BackColor != Color.Red)
            {
                betoltve.BackColor = Color.Red;
                pirosgomb.Start();
            }
        }

        void pirosgomb_Tick(object sender, EventArgs e)
        {
            //------------------------disable other apps to run & load village data
            pirosgomb.Stop();
            mehet = false;
            Betoltes();
        }

        private void Betoltes()
        {
            //------------------------Villages data load/reload
            string seged;
            Falvak[] seged_falvak = new Falvak[1000]; //temp array for max 1000 villages
            int i = 0; //index for the array
            int id = 0; //village's ID
            string nev = ""; //village's name
            string koord = ""; //village's koord
            //------------------------get data to the temp array
            foreach (HtmlElement element in web.Document.GetElementsByTagName("SPAN"))
            {
                seged = element.GetAttribute("id");
                try
                {
                    seged = seged.Substring(0, 11);
                }
                catch (ArgumentOutOfRangeException)
                { }
                if (seged == "label_text_")
                {
                    seged = element.GetAttribute("id");
                    id = Convert.ToInt32(seged.Substring(11));
                    seged = element.GetAttribute("innerHTML");
                    nev = seged.Substring(0, seged.IndexOf("(") - 1);
                    seged = element.GetAttribute("innerHTML");
                    koord = seged.Substring(seged.IndexOf("(") + 1, seged.IndexOf(")") - seged.IndexOf("(") - 1);
                    seged_falvak[i] = new Falvak(id, nev, koord);
                    i++;
                }
            }
            //------------------------Count the villages
            i = 0;
            while (seged_falvak[i] != null)
                i++;
            //------------------------Permament villages array
            falvak = new Falvak[i];
            for (int j = 0; j < i; j++)
                falvak[j] = seged_falvak[j];
            //------------------------Storage not max population villages
            if (File.Exists("nm.txt"))
                File.Delete("nm.txt"); //because sometimes the program f*cked up and double saved the datas
            StreamWriter sw = new StreamWriter("nm.txt", false);
            int s1, s2; //population
            int index = 0; //village index
            foreach (HtmlElement element in web.Document.GetElementsByTagName("TD"))
            {
                string seged_nm;
                seged_nm = element.GetAttribute("innerHTML");
                if (seged_nm != "" && seged_nm.Length <= 12 && seged_nm.IndexOf('/') > 1)
                {
                    index++;
                    s1 = Convert.ToInt32(seged_nm.Substring(0, seged_nm.IndexOf('/')));
                    s2 = Convert.ToInt32(seged_nm.Substring(seged_nm.IndexOf('/') + 1));
                    if (s1 != s2)
                        sw.WriteLine(index);
                }
            }
            sw.Close();
            //------------------------Data loaded -> color red -> green
            betoltve.BackColor = Color.LightGreen;
            mehet = true;
            web.Navigate("http://" + "hu17.klanhaboru.hu/game.php?village=" + falvak[0].id + "&screen=overview_villages");
        }
        #endregion

        #region Navigate_left_right
        private string bbb_jjj(string merre)
        {
            //------------------------Navigate
            //------------------------Get the current village's ID
            string seged1 = url.Text.Substring(0, 43); //begin
            string seged2 = url.Text.Substring(48); //end
            if (seged2.Substring(0, 1) != "&") seged2 = url.Text.Substring(49);
            string seged3 = url.Text.Substring(43, 6); //id
            if (seged3.Substring(5, 1) == "&") seged3 = url.Text.Substring(43, 5);
            int i = 0;
            int seged4 = 0;
            int faluseged = Convert.ToInt32(seged3);
            while ((faluseged != falvak[i].id) && (i < falvak.Length))
                i++;
            if (merre == "balra") //Navigate left
            {
                //------------------------Set the (current - 1) village's ID
                if (i < falvak.Length)
                {
                    if (i - 1 >= 0)
                        seged4 = falvak[i - 1].id;
                    else
                        seged4 = falvak[falvak.Length - 1].id;
                }
            }
            else if (merre == "jobbra") //Navigate right
            {
                //------------------------Set the (current + 1) village's ID
                if (i < falvak.Length)
                {
                    if (i + 1 < falvak.Length)
                        seged4 = falvak[i + 1].id;
                    else
                        seged4 = falvak[0].id;
                }
            }
            string seged5 = seged1 + seged4 + seged2; //URL for navigate
            return seged5;
        }


        void bbb_Click(object sender, EventArgs e)
        {
            //------------------------Navigate left
            web.Navigate(bbb_jjj("balra"));
        }

        void jjj_Click(object sender, EventArgs e)
        {
            //------------------------Navigate right
            web.Navigate(bbb_jjj("jobbra"));
        }
        #endregion

        void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            url.Text = web.Url.ToString();
        }

        private void Bongeszo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
