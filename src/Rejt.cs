using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace bot_v4
{
    class Rejt
    {
        public Rejt(Button[] gombok, Button kivalto, string legyen)
        {
            if (legyen == "rejt")
            {
                for (int i = 0; i < gombok.Length; i++)
                    if (gombok[i] != kivalto && i != 5 && i != 6) //i:5,6 = temp buttons
                        gombok[i].Enabled = false;
                kivalto.BackColor = Color.Red;
                kivalto.Text = "Stop";
            }
            else if (legyen == "mutat")
            {
                kivalto.BackColor = Color.LightGreen;
                for (int i = 0; i < gombok.Length; i++)
                    if (gombok[i] != kivalto && i != 5 && i != 6) //i:5,6 = temp buttons
                        gombok[i].Enabled = true;
                    else if (i == 5 || i == 6) //i:5,6 = temp buttons
                        if (gombok[i] != null)
                        {
                            gombok[i].Visible = false;
                            gombok[i].Enabled = false;
                        }

            }
        }
    }
}
