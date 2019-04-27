using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Timer
    {
        public static double convertion_microseconds;

        public static void setFrequency()
        {
            switch (GUIController.frequency_string + 1)
            {
                case 1:
                    convertion_microseconds = 122.070312;  break; // 32.xx KHz
                case 2:
                    convertion_microseconds = 8.0; break;   // 0.5 MHz
                case 3:
                    convertion_microseconds = 1.0; break;   // 1 MHz
                case 4:
                    convertion_microseconds = 1.0; break;   // 2MHz
                case 5:
                    convertion_microseconds = 1.0; break;   // 2.xx MHz
                case 6:
                    convertion_microseconds = 1.0; break;   // 3 MHz
                case 7: 
                    convertion_microseconds = 1.0; break;    // 3.xx MHz
                case 8:
                    convertion_microseconds = 1.0; break;   // 3.yy MHz
                case 9:
                    convertion_microseconds = 1.0; break;   // 3.zz Mhz
                case 10:
                    convertion_microseconds = 1.0; break;   // 4 MHz
//=======================================================
                case 11:
                    convertion_microseconds = 1.0; break;
                case 12:
                    convertion_microseconds = 1.0; break;
                case 13:
                    convertion_microseconds = 1.0; break;
                case 14:
                    convertion_microseconds = 1.0; break;
                case 15:
                    convertion_microseconds = 1.0; break;
                case 16:
                    convertion_microseconds = 1.0; break;
                case 17:
                    convertion_microseconds = 1.0; break;
                case 18:
                    convertion_microseconds = 1.0; break;
                case 19:
                    convertion_microseconds = 0.5; break;   // 8 MHz
                case 20:
                    convertion_microseconds = 1.0; break;
//=======================================================
                case 21:
                    convertion_microseconds = 1.0; break;
                case 22:
                    convertion_microseconds = 0.25; break;   // 16 Hz
                case 23:
                    convertion_microseconds = 1.0; break;
                case 24:
                    convertion_microseconds = 1.0; break;
                case 25:
                    convertion_microseconds = 0.125; break;  // 32 MHz
                case 26:
                    convertion_microseconds = 0.1; break;    // 40 MHz
                case 27:
                    convertion_microseconds = 0.05; break;   // 80 MHz
//=======================================================
                default:
                    convertion_microseconds = 69; break;
            }
        }
}
