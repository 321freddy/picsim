using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class BTFSS : Command
    {
        public BTFSS(ushort opcode) : base(opcode)
        {
            Debug.Log("BCF");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1100_0000_0000) == 0b0001_1100_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running BTFSS");
        }
    }
}
