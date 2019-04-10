﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class XORLW : Command
    {
        private byte literal;

        public XORLW(ushort opcode) : base(opcode)
        {
            literal = (byte)(opcode & 0b1111_1111);
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_0000_0000) == 0b0011_1010_0000_0000;
        }

        public override void run(Memory memory)
        {
            Debug.Log("running XORLW");
            memory.w_Register = (byte) (literal ^ memory.w_Register);
            base.run(memory); // Increase PC
        }
    }
}
