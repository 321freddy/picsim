using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class SWAPF : Command
    {
        private byte address;
        private bool writeToMemory;

        public SWAPF(ushort opcode, int line) : base(opcode, line)
        {
            address = (byte)Bit.mask(opcode, 7);
            writeToMemory = Bit.get(opcode, 7) == 1;
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1110_0000_0000;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running SWAPF");
            var variable = memory[address];
            var lower = Bit.get(variable, 0, 4);
            var upper = Bit.get(variable, 4, 4);
            var result = (lower << 4) + upper;

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
