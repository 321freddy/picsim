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

        public SUBLW(ushort opcode, int line) : base(opcode, line)
        {
            literal = (byte)Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1110_0000_0000) == 0b0011_1100_0000_0000;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running SUBLW");
            int result = literal - memory.w_Register;

            if (result >= 0)
            {
                memory.Carry = 1;
            }
            else
            {
                memory.Carry = 0;
            }

            if (Bit.mask(literal, 4) - Bit.mask(memory.w_Register, 4) >= 0)
            {
                memory.DigitCarry = 1;
            }
            else
            {
                memory.DigitCarry = 0;
            }

            // Update Zero Flag
            if (((byte)result) == 0)
            {
                memory.ZeroFlag = 1;
            }
            else
            {
                memory.ZeroFlag = 0;
            }

            memory.w_Register = (byte) result;
        }
    }
}
