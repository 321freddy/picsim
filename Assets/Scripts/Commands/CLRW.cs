using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class CLRW : Command
    {
        public CLRW(ushort opcode) : base(opcode)
        {

        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_1000_0000) == 0b00_0001_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running CLRW");
            memory.w_Register = 0;
            memory.ZeroFlag = 1;
            base.run(memory); // Increase PC
        }
    }
}
