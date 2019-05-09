﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class IORLW : Command
    {
        private byte literal;

        public IORLW(ushort opcode, int line) : base(opcode, line)
        {
            literal = (byte) Bit.mask(opcode, 8);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_0000_0000) == 0b0011_1000_0000_0000;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running IORLW");
            memory.w_Register = (byte) (literal | memory.w_Register);

            // Update Zero Flag
            if (memory.w_Register == 0)
            {
                memory.ZeroFlag = 1;
            }
            else
            {
                memory.ZeroFlag = 0;
            }

        }
    }
}
