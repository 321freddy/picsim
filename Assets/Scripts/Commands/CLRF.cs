using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class CLRF : Command
    {
        private byte address;

        public CLRF(ushort opcode) : base(opcode)
        {
            address = (byte)Bit.mask(opcode, 7);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_1000_0000) == 0b00_0001_1000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running CLRF");
            memory[address] = 0;
            memory.ZeroFlag = 1;
            base.run(memory); // Increase PC
        }
    }
}
