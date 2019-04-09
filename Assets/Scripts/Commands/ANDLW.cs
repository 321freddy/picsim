using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class ANDLW : Command
    {

        public ANDLW(string command) : base(command)
        {
            int literal;
            literal = Convert.ToInt32(command, 16) ^ 0b11_1001_0000_0000; //extract last 8 bits
            MOVLW.w_Register = literal & MOVLW.w_Register;
            Debug.Log("W-Register: " + MOVLW.w_Register.ToString("X") + " HEX");
            Debug.Log("ANDLW");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_0000_0000) == 0b0011_1001_0000_0000;
        }

        public override void run()
        {
            Debug.Log("running ANDLW");
        }
    }
}
