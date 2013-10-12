using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_v4
{
    class Falvak
    {
        public int id;
        public string falunev;
        public string koord;

        public Falvak(int id, string falunev, string koord)
        {
            this.id = id;
            this.falunev = falunev;
            this.koord = koord;
        }
    }
}
