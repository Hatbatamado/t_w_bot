using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace bot_v4
{
    public partial class Bongeszo : Form
    { 
        #region Vars
        private static Random R = new Random();
        private static WebBrowser web;
        private string data; //user login data
        private static bool botvedelem = false; //bot protection
        private static bool mehet = false; //enables/disables the specific bot apps to run
        private static Falvak[] falvak; //villages data: ID, name, koord
        private static int kattintas = 0; //nm_button click count
        private static int[] segednem2; //temp array for sorted villages
        #endregion       
        //------------------------GUI vars
        #region GUI vars
        private static Label url = new Label(); //url label
        //------------------------
        private static CheckBox tamado = new CheckBox(); //village type: attacker
        private static CheckBox vedo = new CheckBox(); //village type: defender
        private static CheckBox kem = new CheckBox(); //village type: spy
        private static CheckBox ddd = new CheckBox(); //market checkbox
        //------------------------
        private static Button betoltve = new Button(); //villages data load button
        private static Button bbb = new Button(); //navigate left
        private static Button jjj = new Button(); //navigate right
        private static Button ok = new Button(); //village type change confirm
        private static Button nm = new Button(); //not max population villages
        private static Button ujbal = new Button(); //sorted villages button (left)
        private static Button ujjobb = new Button(); //sorted villages button (right)        
        //------------------------
        private static Rejt rejt;
        private static Village_ID vi;
        //------------------------
        private static Button[] gombok = new Button[7];

        private void Gombok_fel()
        {
            gombok[0] = betoltve;
            gombok[1] = bbb;
            gombok[2] = jjj;
            gombok[3] = ok;
            gombok[4] = nm;
            gombok[5] = ujbal;
            gombok[6] = ujjobb;
        }
        #endregion

        public Bongeszo(string data)
        {
            InitializeComponent();
            //------------------------
            Registry.SetValue("HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current", "", "NULL"); //IE mute
            //Registry.SetValue("HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current","","C:\Windows\Media\Cityscape\Windows Navigation Start.wav"); //IE unmute
            //------------------------Window max height, half width
            this.Size = new System.Drawing.Size(Screen.GetWorkingArea(this).Width / 2, Screen.GetWorkingArea(this).Height);
            this.Location = new Point(0, 0);
            //------------------------
            this.data = data;
            Gombok_fel();
            Timer_init();

            Belep();
        }

        #region Timers
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
        #endregion

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

        private void belep5_Tick(object sender, EventArgs e)
        {
            //------------------------Navigate to villages list page
            belep[4].Stop();
            web.Navigate("http://hu17.klanhaboru.hu/game.php?screen=overview_villages&intro");
            //------------------------
            belep[5].Start();
        }
        #endregion

        #region GUI
        private void belep6_Tick(object sender, EventArgs e)
        {
            //------------------------GUI setup
            belep[5].Stop();
            //------------------------bot protection check
            if (web.Document.GetElementById("bot_check_image") != null)
                botvedelem = true;
            else
            {
                //------------------------GUI
                #region GUI_init
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
                //------------------------Village type: attacker
                Controls.Add(tamado);
                tamado.Text = "támadó";
                tamado.Location = new Point(10, 24);
                tamado.Size = new System.Drawing.Size(66, 22);
                tamado.CheckedChanged += new EventHandler(tamado_CheckedChanged);
                //------------------------Village type: defender
                Controls.Add(vedo);
                vedo.Text = "védő";
                vedo.Location = new Point(80, 24);
                vedo.Size = new System.Drawing.Size(50, 22);
                vedo.CheckedChanged += new EventHandler(vedo_CheckedChanged);
                //------------------------Village type: spy
                Controls.Add(kem);
                kem.Text = "kém";
                kem.Location = new Point(140, 24);
                kem.Size = new System.Drawing.Size(46, 22);
                kem.CheckedChanged += new EventHandler(kem_CheckedChanged);
                //------------------------Village type changed confirm
                Controls.Add(ok);
                ok.Text = "OK";
                ok.Location = new Point(185, 24);
                ok.Size = new Size(30, 22);
                ok.Click += new EventHandler(ok_Click);
                //------------------------Not max population villages
                Controls.Add(nm);
                nm.Text = "Nemmax";
                nm.Location = new Point(230, 24);
                nm.Size = new System.Drawing.Size(60, 22);
                nm.BackColor = Color.LightGreen;
                nm.Click += new EventHandler(nm_Click);
                //------------------------Market
                Controls.Add(ddd);
                ddd.Text = "piac";
                ddd.Location = new Point(500, 4);
                ddd.Size = new System.Drawing.Size(50, 22);
                ddd.Checked = false;
                ddd.CheckedChanged += new EventHandler(ddd_CheckedChanged);
                #endregion
                pirosgomb.Start();
            }
        }
        #region Market checkbox
        private void ddd_CheckedChanged(object sender, EventArgs e)
        {
            //------------------------Market
            if (ddd.Checked)
            {
                url.Text = web.Url.ToString(); //update the url
                Piac();
            }
        }

        private void Piac()
        {
            //------------------------Market
            //------------------------resources equator
            int fa = 0, agyag = 0, vas = 0; //resources
            string seged = url.Text.Substring(url.Text.Length - 21); //url end
            //------------------------
            if (web.Document.GetElementById("wood") != null && web.Document.GetElementById("stone") != null && web.Document.GetElementById("iron") != null)
            {
                fa = Convert.ToInt32(web.Document.GetElementById("wood").GetAttribute("innerHTML")) - 50; //-50 so won't all the tree equal
                agyag = Convert.ToInt32(web.Document.GetElementById("stone").GetAttribute("innerHTML")) - 100; //-100 so won't all the tree equal
                vas = Convert.ToInt32(web.Document.GetElementById("iron").GetAttribute("innerHTML"));
            }
            //------------------------
            if (seged == "market&mode=own_offer")
            {
                //------------------------Market own offer
                //------------------------Standard value: 1000
                foreach (HtmlElement element in web.Document.GetElementsByTagName("INPUT"))
                {
                    if (element.GetAttribute("name") == "sell")
                    {
                        element.SetAttribute("value", Convert.ToString(1000));
                    }
                }
                //------------------------Standard value: 1000
                foreach (HtmlElement element in web.Document.GetElementsByTagName("INPUT"))
                {
                    if (element.GetAttribute("name") == "buy")
                    {
                        element.SetAttribute("value", Convert.ToString(1000));
                    }
                }
                if (web.Document.GetElementById("res_sell_wood") != null && web.Document.GetElementById("res_sell_stone") != null && web.Document.GetElementById("res_sell_iron") != null)
                {
                    //------------------------Max value
                    if (fa > agyag && fa > vas)
                        web.Document.GetElementById("res_sell_wood").InvokeMember("click");
                    else if (agyag > fa && agyag > vas)
                        web.Document.GetElementById("res_sell_stone").InvokeMember("click");
                    else if (vas > fa && vas > agyag)
                        web.Document.GetElementById("res_sell_iron").InvokeMember("click");
                    //------------------------Min value
                    if (fa < agyag && fa < vas)
                        web.Document.GetElementById("res_buy_wood").InvokeMember("click");
                    else if (agyag < fa && agyag < vas)
                        web.Document.GetElementById("res_buy_stone").InvokeMember("click");
                    else if (vas < fa && vas < agyag)
                        web.Document.GetElementById("res_buy_iron").InvokeMember("click");
                }
            }
            else if (seged == "rket&mode=other_offer")
            {
                //------------------------Market other offer
                foreach (HtmlElement element in web.Document.GetElementsByTagName("SELECT"))
                {
                    //------------------------Min value
                    if (element.GetAttribute("name") == "res_sell")
                    {
                        if (fa < agyag && fa < vas)
                            element.SetAttribute("value", "wood");

                        else if (agyag < fa && agyag < vas)
                            element.SetAttribute("value", "stone");
                        else if (vas < fa && vas < agyag)
                            element.SetAttribute("value", "iron");
                    }
                    //------------------------Max value
                    if (element.GetAttribute("name") == "res_buy")
                    {
                        if (fa > agyag && fa > vas)
                        {
                            element.SetAttribute("value", "wood");
                            break;
                        }
                        else if (agyag > fa && agyag > vas)
                        {
                            element.SetAttribute("value", "stone");
                            break;
                        }
                        else if (vas > fa && vas > agyag)
                        {
                            element.SetAttribute("value", "iron");
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region not max pop
        private void nm_Click(object sender, EventArgs e)
        {
            //------------------------Sort the not max population villages
            //------------------------Enable/Disable buttons
            kattintas++;
            if (!(kattintas % 2 == 0))
            {
                rejt = new Rejt(gombok, nm, "rejt");
                NM();
            }
            else
            {
                nm.Text = "Nemmax";
                rejt = new Rejt(gombok, nm, "mutat");
            }
        }

        private void NM()
        {
            //------------------------Sort the not max population villages
            int i = 0;
            int[] seged = new int[1000]; //temp array for max 1000 villages
            StreamReader sr = new StreamReader("nm.txt");
            while (!sr.EndOfStream)
            {
                seged[i] = Convert.ToInt32(sr.ReadLine());
                i++;
            }
            sr.Close();
            //------------------------
            int[] segednem = new int[i]; //temp array with perm. No.
            for (int j = 0; j < i; j++)
                segednem[j] = seged[j];
            //------------------------
            segednem2 = new int[i]; //perm. array
            for (int j = 1; j <= segednem.Length; j++)
                segednem2[j - 1] = falvak[segednem[j - 1] - 1].id;
            //------------------------Navigate left
            Controls.Add(ujbal);
            ujbal.Visible = true;
            ujbal.Enabled = true;
            ujbal.Text = "<-";
            ujbal.Size = new System.Drawing.Size(30, 22);
            ujbal.Location = new Point(340, 24);
            ujbal.Click += new EventHandler(ujbal_Click);
            //------------------------Navigate right
            Controls.Add(ujjobb);
            ujjobb.Visible = true;
            ujjobb.Enabled = true;
            ujjobb.Text = "->";
            ujjobb.Size = new System.Drawing.Size(30, 22);
            ujjobb.Location = new Point(380, 24);
            ujjobb.Click += new EventHandler(ujjobb_Click);
            //------------------------Navigate to the 1st not max
            string seged1 = url.Text.Substring(0, 43); //begin
            string seged2 = url.Text.Substring(48); //end
            string seged4 = segednem2[0].ToString();
            string seged5 = seged1 + seged4 + seged2;
            web.Navigate(seged5);
        }

        private string ujb_ujj(string merre, int[] segednem2)
        {
            //------------------------Navigate
            vi = new Village_ID(url.Text, null, segednem2, merre);
            return vi.seged5;
        }

        private void ujjobb_Click(object sender, EventArgs e)
        {
            //------------------------Navigate right
            web.Navigate(ujb_ujj("jobbra", segednem2));
        }

        private void ujbal_Click(object sender, EventArgs e)
        {
            //------------------------Navigate left
            web.Navigate(ujb_ujj("balra", segednem2));
        }
        #endregion

        #region betoltve
        private void betoltve_Click(object sender, EventArgs e)
        {
            //------------------------Color check, always should run without any problem
            if (betoltve.BackColor != Color.Red)
            {
                betoltve.BackColor = Color.Red;
                pirosgomb.Start();
            }
        }

        private void pirosgomb_Tick(object sender, EventArgs e)
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
            web.Navigate("http://" + "hu17.klanhaboru.hu/game.php?village=" + falvak[0].id +
                "&screen=overview_villages");
        }
        #endregion

        #region Navigate_left_right
        private string bbb_jjj(string merre)
        {
            //------------------------Navigate
            vi = new Village_ID(url.Text, falvak, null, merre);
            return vi.seged5;
        }

        private void bbb_Click(object sender, EventArgs e)
        {
            //------------------------Navigate left
            web.Navigate(bbb_jjj("balra"));
        }

        private void jjj_Click(object sender, EventArgs e)
        {
            //------------------------Navigate right
            web.Navigate(bbb_jjj("jobbra"));
        }
        #endregion

        #region village types + save + change
        private void ok_Click(object sender, EventArgs e)
        {
            //------------------------Save / change the checked village type
            string[] segedfalvak = Attolt(); //Load village types if exists
            string seged = url.Text.Substring(43, 6); //id
            if (seged.Substring(5, 1) == "&") seged = url.Text.Substring(43, 5); //id
            string seged3 = "";
            if (tamado.Checked)
                seged3 = "támadó";
            else if (vedo.Checked)
                seged3 = "védő";
            else if (kem.Checked)
                seged3 = "kém";
            if (seged3 != "")
            {
                string seged2 = "";
                if (File.Exists("tipus.txt"))
                {
                    StreamReader sr = new StreamReader("tipus.txt");
                    int i = 0;
                    while (!sr.EndOfStream)
                    {
                        seged2 = sr.ReadLine();
                        string[] feloszt = seged2.Split(' ');
                        if (feloszt[0] == seged)
                        {
                            sr.Close();
                            string[] segedf;
                            for (int b = 0; b < segedfalvak.Length; b++)
                            {
                                segedf = segedfalvak[b].Split(' ');
                                if (feloszt[0] == segedf[0])
                                    segedfalvak[b] = feloszt[0] + " " + seged3;
                            }
                            StreamWriter sw = new StreamWriter("tipus.txt", false);
                            for (int c = 0; c < segedfalvak.Length; c++)
                            {
                                sw.WriteLine(segedfalvak[c]);
                            }
                            sw.Close();
                            break;
                        }
                        i++;
                    }
                    sr.Close();
                    if (i == segedfalvak.Length)
                    {
                        StreamWriter sw = new StreamWriter("tipus.txt", true);
                        sw.WriteLine(seged + " " + seged3);
                        sw.Close();
                    }
                }
                else
                {
                    StreamWriter sw = new StreamWriter("tipus.txt");
                    sw.WriteLine(seged + " " + seged3);
                    sw.Close();
                }
            }
        }

        private string[] Attolt()
        {
            //------------------------Load village types if exists
            if (File.Exists("tipus.txt"))
            {
                int i = 0;
                string[] seged = new string[1000]; //temp array for max 1000 villages
                StreamReader sr = new StreamReader("tipus.txt");
                while (!sr.EndOfStream)
                {
                    seged[i] = sr.ReadLine();
                    i++;
                }
                sr.Close();
                string[] segedfalvak = new string[i];
                for (int j = 0; j < i; j++)
                    segedfalvak[j] = seged[j];
                return segedfalvak;
            }
            else
                return null;
        }

        private void kem_CheckedChanged(object sender, EventArgs e)
        {
            if (kem.Checked)
            {
                tamado.Checked = false;
                vedo.Checked = false;
            }
        }

        private void vedo_CheckedChanged(object sender, EventArgs e)
        {
            if (vedo.Checked)
            {
                tamado.Checked = false;
                kem.Checked = false;
            }
        }

        private void tamado_CheckedChanged(object sender, EventArgs e)
        {
            if (tamado.Checked)
            {
                vedo.Checked = false;
                kem.Checked = false;
            }
        }
        #endregion
        #endregion

        private void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            url.Text = web.Url.ToString();
            if (botvedelem)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"dinding.wav");
                player.Play();
                botvedelem = false;
            }
            else if (mehet && !botvedelem)
            {
                //------------------------Set the village types
                #region vill_type
                if (File.Exists("tipus.txt"))
                {
                    vi = new Village_ID(url.Text, null, null, null);
                    string seged = vi.seged3.ToString();
                    string seged2;
                    string[] seged3;
                    bool megvan = false;
                    StreamReader sr = new StreamReader("tipus.txt");
                    while (!sr.EndOfStream)
                    {
                        seged2 = sr.ReadLine();
                        seged3 = seged2.Split(' ');
                        if (seged == seged3[0])
                        {
                            if (seged3[1] == "támadó")
                                tamado.Checked = true;
                            else if (seged3[1] == "védő")
                                vedo.Checked = true;
                            else if (seged3[1] == "kém")
                                kem.Checked = true;
                            megvan = true;
                        }
                    }
                    sr.Close();
                    if (!megvan)
                    {
                        tamado.Checked = false;
                        vedo.Checked = false;
                        kem.Checked = false;
                    }
                }
                #endregion
                //------------------------
            }
        }

        private void Bongeszo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
