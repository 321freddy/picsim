using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RETURN : Command
    {
        public RETURN(ushort opcode, int line) : base(opcode, line)
        {

        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0000_1000;
        }

        protected override int updateProgramCounter(Memory memory)
        {
            Debug.Log("running RETURN");
            memory.ProgramCounter = memory.popStack();
            return 2;
        }
    }
}
