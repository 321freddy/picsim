using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class GOTO : Command
    {
        private ushort literal;

        public GOTO(ushort opcode) : base(opcode)
        {
            literal = (ushort) Bit.mask(opcode, 11);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1000_0000_0000) == 0b0010_1000_0000_0000;
        }

        protected override int updateProgramCounter(Memory memory)
        {
            Debug.Log("running GOTO");
            memory.ProgramCounter = literal;
            return 2;
        }
    }
}
