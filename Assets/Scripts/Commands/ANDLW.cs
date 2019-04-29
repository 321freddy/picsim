using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class ANDLW : Command
    {
        private byte literal;

        public ANDLW(ushort opcode) : base(opcode)
        {
            literal = (byte) Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_0000_0000) == 0b0011_1001_0000_0000;
        }

        public override int run(Memory memory)
        {
            Debug.Log("running ANDLW");
            memory.w_Register = (byte) (literal & memory.w_Register);

            // Update Zero Flag
            if (memory.w_Register == 0)
            {
                memory.ZeroFlag = 1;
            }
            else
            {
                memory.ZeroFlag = 0;
            }

            memory.ProgramCounter++;
            return base.run(memory);
        }
    }
}
