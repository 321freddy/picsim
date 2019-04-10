using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class COMF : Command
    {
        public COMF(ushort opcode) : base(opcode)
        {
            Debug.Log("COMF");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1001_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running COMF");
        }
    }
}
