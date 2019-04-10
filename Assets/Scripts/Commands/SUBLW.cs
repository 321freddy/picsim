using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class SUBLW : Command
    {
        private byte literal;

        public SUBLW(ushort opcode) : base(opcode)
        {
            literal = (byte) (opcode & 0b1111_1111);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1110_0000_0000) == 0b0011_1100_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running SUBLW");
            memory.w_Register = (byte) (literal - memory.w_Register);
            base.run(memory); // Increase PC
        }
    }
}
