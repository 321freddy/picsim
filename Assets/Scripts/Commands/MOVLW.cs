using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class MOVLW : Command
    {
        public MOVLW(string command) : base(command)
        {
            Memory.w_Register = Convert.ToInt32(command, 16) ^ 0b11_0000_0000_0000; //extract last 8 bits

            Debug.Log("W-Register: " + Memory.w_Register.ToString("X") + " HEX");
            Debug.Log("Zero-Flag: " + Memory.zero_Flag);
            Debug.Log("MOVLW");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1100_0000_0000) == 0b11_0000_0000_0000;
        }

        public override void run()
        {
            Debug.Log("running MOVLW");
        }
    }
}
