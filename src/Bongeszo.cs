using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bot_v4
{
    public partial class Bongeszo : Form
    {
        static Random R = new Random();
        static WebBrowser web;
        private string data; //user login data

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

        static Timer[] belep = new Timer[6];
        private void Timer_init()
        {
            //------------------------Timers initalize
            for (int i = 0; i < belep.Length; i++)
                belep[i] = new Timer();
            //------------------------1) webbrowser setup
            belep[0].Interval = R.Next(250, 500);
            belep[0].Tick += new EventHandler(belep1_Tick);
            //------------------------2) website load for login info
            //for slower connection, so the program won't crash
            //document_completed could do this too, but for now this is enough
            //TODO: document_completed
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
        }

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

        void belep6_Tick(object sender, EventArgs e)
        {
            //------------------------
            belep[5].Stop();
        }

        void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }

        private void Bongeszo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
