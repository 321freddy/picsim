﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class BSF : Command
    {
        private byte address;
        private int bit;

        public BSF(ushort opcode) : base(opcode)
        {
            address = (byte)Bit.mask(opcode, 7);
            bit = Bit.get(opcode, 7, 3);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1100_0000_0000) == 0b0001_0100_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running BSF");
            memory[address] = (byte)Bit.set(memory[address], bit);
            base.run(memory); // Increase PC
        }
    }
}
