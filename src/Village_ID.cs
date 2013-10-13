using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_v4
{
    class Village_ID
    {
        //------------------------Get the current village's ID
        public int seged;
        public string seged1;
        public string seged2;
        public Village_ID(string url)
        {
            string seged1 = url.Substring(0, 43); //begin
            string seged2 = url.Substring(48); //end
            if (seged2.Substring(0, 1) != "&") seged2 = url.Substring(49);
            string seged3 = url.Substring(43, 6); //id
            if (seged3.Substring(5, 1) == "&") seged3 = url.Substring(43, 5);
            seged = Convert.ToInt32(seged3);
            this.seged1 = seged1;
            this.seged2 = seged2;
        }
    }
}
