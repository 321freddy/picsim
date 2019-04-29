﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class BTFSC : Command
    {
        private byte address;
        private int bit;

        public BTFSC(ushort opcode) : base(opcode)
        {
            address = (byte)Bit.mask(opcode, 7);
            bit = Bit.get(opcode, 7, 3);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1100_0000_0000) == 0b0001_1000_0000_0000;
        }

        public override int run(Memory memory)
        {
            Debug.Log("running BTFSC");
            
            if (Bit.get(memory[address], bit) == 0)
            {
                memory.ProgramCounter += 2;
                base.run(memory);
                return 2;
            }

            base.run(memory);
            return 1;
        }
    }
}
