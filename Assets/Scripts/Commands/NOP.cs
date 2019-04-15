using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class NOP : Command
    {
        public NOP(ushort opcode) : base(opcode)
        {

        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_1001_1111) == 0;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running NOP");
            base.run(memory); // Increase PC
        }
    }
}
