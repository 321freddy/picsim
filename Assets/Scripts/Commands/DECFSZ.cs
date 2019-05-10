using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class DECFSZ : Command
    {
        private byte address;
        private bool writeToMemory;

        public DECFSZ(ushort opcode, int line) : base(opcode, line)
        {
            address = (byte)Bit.mask(opcode, 7);
            writeToMemory = Bit.get(opcode, 7) == 1;
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_0000_0000) == 0b00_1011_0000_0000;
        }

        protected override int updateProgramCounter(Memory memory)
        {
            Debug.Log("running DECFSZ");
            int result = memory[address] - 1;

            if (writeToMemory)
            {
                memory[address] = (byte)result;
            }
            else
            {
                memory.w_Register = (byte)result;
            }

            // Skip if zero
            if (((byte)result) == 0)
            {
                memory.ProgramCounter += 2;
                return 2;
            }

            memory.ProgramCounter++;
            return 1;
        }
    }
}
