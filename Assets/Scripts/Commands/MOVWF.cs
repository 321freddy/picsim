using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class MOVWF : Command
    {
        private byte address;

        public MOVWF(ushort opcode) : base(opcode)
        {
            address = (byte) Bit.mask(opcode, 7);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_1000_0000) == 0b1000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running MOVWF");
            memory[address] = memory.w_Register;
            base.run(memory); // Increase PC
        }
    }
}
