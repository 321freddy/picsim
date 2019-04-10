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
        private byte literal;

        public MOVLW(ushort opcode) : base(opcode)
        {
            literal = (byte) (opcode & 0b1111_1111);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1100_0000_0000) == 0b11_0000_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running MOVLW");
            memory.w_Register = literal;
            base.run(memory); // Increase PC
        }
    }
}
