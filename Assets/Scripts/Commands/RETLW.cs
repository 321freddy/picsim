using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RETLW : Command
    {
        private byte literal;

        public RETLW(ushort opcode, int line) : base(opcode, line)
        {
            literal = (byte) Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1100_0000_0000) == 0b0011_0100_0000_0000;
        }

        protected override int updateProgramCounter(Memory memory)
        {
            Debug.Log("running RETLW");
            memory.w_Register = literal;
            memory.ProgramCounter = memory.popStack();
            return 2;
        }
    }
}
