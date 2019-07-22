using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay
{
    static class Judgement
    {
        static int max = 16;
        static int perfect = 25;
        static int great = 40;
        static int good = 60;
        static int bad = 100;
        static int miss = 130;

        static public KeyValuePair<double,int> getErrorJudgement(int error)
        {
            KeyValuePair<double, int> result = new KeyValuePair<double, int>(-1,-1);
            if (error < miss)
            {
                if(error < max)
                {
                    result = new KeyValuePair<double, int>(1, 320);
                }
                else if (error < perfect)
                {
                    result = new KeyValuePair<double, int>(1, 300);
                }
                else if (error < great)
                {
                    result = new KeyValuePair<double, int>(2/3, 200);
                }
                else if (error < good)
                {
                    result = new KeyValuePair<double, int>(1/3, 100);
                }
                else
                {
                    result = new KeyValuePair<double, int>(0, 0);
                }
            }

            return result;
        }
    }
}
