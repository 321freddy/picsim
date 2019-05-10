using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class MOVF : Command
    {
        private byte address;
        private bool writeToMemory;

        public MOVF(ushort opcode, int line) : base(opcode, line)
        {
            address = (byte)Bit.mask(opcode, 7);
            writeToMemory = Bit.get(opcode, 7) == 1;
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1000_0000_0000;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running MOVF");
            var result = memory[address];

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

        }
    }
}
