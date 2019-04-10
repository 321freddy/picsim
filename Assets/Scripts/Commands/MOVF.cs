using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class MOVF : Command
    {
        public MOVF(ushort opcode) : base(opcode)
        {
            Debug.Log("MOVF");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1000_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running MOVF");
        }
    }
}
