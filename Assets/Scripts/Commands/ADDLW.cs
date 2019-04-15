using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class ADDLW : Command
    {
        private byte literal;

        public ADDLW(ushort opcode) : base(opcode)
        {
            literal = (byte) Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1110_0000_0000) == 0b0011_1110_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running ADDLW");
            int result = literal + memory.w_Register;
            
            if (result > 0xFF)
            {
                memory.Carry = 1;
            }
            else
            {
                memory.Carry = 0;
            }

            if (Bit.mask(literal, 4) + Bit.mask(memory.w_Register, 4) > 0xF)
            {
                memory.DigitCarry = 1;
            }
            else
            {
                memory.DigitCarry = 0;
            }

            // Update Zero Flag
            if (result == 0)
            {
                memory.ZeroFlag = 1;
            }
            else
            {
                memory.ZeroFlag = 0;
            }

            memory.w_Register = (byte) result;
            base.run(memory); // Increase PC
        }
    }
}
