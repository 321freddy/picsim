﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class MOVLW : Command
    {
        private byte literal;

        public MOVLW(ushort opcode) : base(opcode)
        {
            literal = (byte) Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1100_0000_0000) == 0b11_0000_0000_0000;
        }

        public override int run(Memory memory)
        {
            Debug.Log("running MOVLW");
            memory.w_Register = literal;

            // Update Zero Flag
            if (literal == 0)
            {
                memory.ZeroFlag = 1;
            }
            else
            {
                memory.ZeroFlag = 0;
            }

            return base.run(memory); // Increase PC
        }
    }
}
