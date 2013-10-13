using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_v4
{
    class Village_ID
    {
        public string seged5;
        public string seged3;
        public Village_ID(string url,Falvak[] falvak, int[]segednem2, string merre)
        {
            //------------------------Get the current village's ID
            string seged1 = url.Substring(0, 43); //begin
            string seged2 = url.Substring(48); //end
            if (seged2.Substring(0, 1) != "&") seged2 = url.Substring(49);
            string seged3 = url.Substring(43, 6); //id
            if (seged3.Substring(5, 1) == "&") seged3 = url.Substring(43, 5);
            //------------------------
            this.seged3 = seged3;
            //------------------------
            int faluseged = Convert.ToInt32(seged3);
            if (falvak != null)
            {
                //------------------------Temp array so we can use one method for both kind of arrays
                int[] seged = new int[falvak.Length];
                for (int j = 0; j < seged.Length; j++)
                    seged[j] = falvak[j].id;
                Tovabb(seged, faluseged, merre, seged1, seged2);
            }
            else if (segednem2 != null)
                Tovabb(segednem2, faluseged, merre, seged1, seged2);
        }

        private void Tovabb(int[] tomb, int faluseged, string merre, string seged1, string seged2)
        {
            int i = 0;
            int seged4 = 0;
            while ((faluseged != tomb[i]) && (i < tomb.Length - 1))
                i++;
            if (merre == "balra") //Navigate left
            {
                //------------------------Set the (current - 1) village's ID
                if (i < tomb.Length)
                {
                    if (i - 1 >= 0)
                        seged4 = tomb[i - 1];
                    else
                        seged4 = tomb[tomb.Length - 1];
                }
            }
            else if (merre == "jobbra") //Navigate right
            {
                //------------------------Set the (current + 1) village's ID
                if (i < tomb.Length)
                {
                    if (i + 1 < tomb.Length)
                        seged4 = tomb[i + 1];
                    else
                        seged4 = tomb[0];
                }
            }
            seged5 = seged1 + seged4 + seged2; //URL for navigate
        }
    }
}
