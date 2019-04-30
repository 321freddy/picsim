﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RLF : Command
    {
        private byte address;
        private bool writeToMemory;

        public RLF(ushort opcode) : base(opcode)
        {
            address = (byte)Bit.mask(opcode, 7);
            writeToMemory = Bit.get(opcode, 7) == 1;
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1101_0000_0000;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running RLF");
            int result = memory[address] << 1;
            result = Bit.setTo(result, 0, memory.Carry);
            memory.Carry = (byte) Bit.get(result, 8);

            if (writeToMemory)
            {
                memory[address] = (byte)result;
            }
            else
            {
                memory.w_Register = (byte)result;
            }

        }
    }
}
