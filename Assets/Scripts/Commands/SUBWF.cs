﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class SUBWF : Command
    {
        private byte address;
        private bool writeToMemory;

        public SUBWF(ushort opcode) : base(opcode)
        {
            address = (byte)Bit.mask(opcode, 7);
            writeToMemory = Bit.get(opcode, 7) == 1;
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_0010_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running SUBWF");
            int result = memory[address] - memory.w_Register;

            if (result >= 0)
            {
                memory.Carry = 1;
            }
            else
            {
                memory.Carry = 0;
            }

            if (Bit.mask(memory[address], 4) - Bit.mask(memory.w_Register, 4) >= 0)
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

            if (writeToMemory)
            {
                memory[address] = (byte)result;
            }
            else
            {
                memory.w_Register = (byte)result;
            }

            base.run(memory); // Increase PC
        }
    }
}
