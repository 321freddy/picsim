using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public static class Timer
    {
        public static double microseconds_per_step;

        public static void setFrequency(int frequencyIndex)
        {
            switch (frequencyIndex + 1)
            {
                case 1:
                    microseconds_per_step = 122.070312;  break;//  0.032768 MHz
                case 2:
                    microseconds_per_step = 8.0; break;        //  0.500000 MHz
                case 3:
                    microseconds_per_step = 4.0; break;        //  1.000000 MHz
                case 4:
                    microseconds_per_step = 2.0; break;        //  2.000000 MHz
                case 5:
                    microseconds_per_step = 1.627604; break;   //  2.457600 MHz
                case 6:
                    microseconds_per_step = 1.333333; break;   //  3.000000 MHz
                case 7: 
                    microseconds_per_step = 1.220703; break;   //  3.276800 MHz
                case 8:
                    microseconds_per_step = 1.086956; break;   //  3.680000 MHz
                case 9:
                    microseconds_per_step = 1.085066; break;   //  3.686411 Mhz
                case 10:
                    microseconds_per_step = 1.0; break;        //  4.000000 MHz
//===============================================================================
                case 11:
                    microseconds_per_step = 0.976562; break;   //  4.096000 MHz
                case 12:
                    microseconds_per_step = 0.953674; break;   //  4.194304 MHz
                case 13:
                    microseconds_per_step = 0.902197; break;   //  4.433619 MHz
                case 14:
                    microseconds_per_step = 0.813802; break;   //  4.915200 MHz
                case 15:
                    microseconds_per_step = 0.8; break;        //  5.000000 MHz
                case 16:
                    microseconds_per_step = 0.666666; break;   //  6.000000 MHz
                case 17:
                    microseconds_per_step = 0.64; break;       //  6.250000 MHz
                case 18:
                    microseconds_per_step = 0.610351; break;   //  6.553600 MHz
                case 19:
                    microseconds_per_step = 0.5; break;        //  8.000000 MHz
                case 20:
                    microseconds_per_step = 0.4; break;        // 10.000000 MHz
//===============================================================================
                case 21:
                    microseconds_per_step = 0.333333; break;   // 12.000000 MHz
                case 22:
                    microseconds_per_step = 0.25; break;       // 16.000000 MHz
                case 23:
                    microseconds_per_step = 0.2; break;        // 20.000000 MHz
                case 24:
                    microseconds_per_step = 0.166666; break;   // 24.000000 MHz
                case 25:
                    microseconds_per_step = 0.125; break;      // 32.000000 MHz
                case 26:
                    microseconds_per_step = 0.1; break;        // 40.000000 MHz
                case 27:
                    microseconds_per_step = 0.05; break;       // 80.000000 MHz
//===============================================================================
                default:
                    microseconds_per_step = 1.0; break;        // 4.000000 MHz 
                                                                 // DEFAULT
            }
        }
}
