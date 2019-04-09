using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class XORLW : Command
    {
        public XORLW(string command) : base(command)
        {
            int literal;

            literal = Convert.ToInt32(command, 16) ^ 0b11_1010_0000_0000; //extract last 8 bits
            if ((literal ^ Memory.w_Register) == 0)
            {
                Memory.zero_Flag = 1;
            }
            else
            {
                Memory.zero_Flag = 0;
            }

            Memory.w_Register = literal ^ Memory.w_Register;

            Debug.Log("W-Register: " + Memory.w_Register.ToString("X") + " HEX");
            Debug.Log("Zero-Flag: " + Memory.zero_Flag);
            Debug.Log("XORLW");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_0000_0000) == 0b0011_1010_0000_0000;
        }

        public override void run()
        {
            Debug.Log("running XORLW");
        }
    }
}
