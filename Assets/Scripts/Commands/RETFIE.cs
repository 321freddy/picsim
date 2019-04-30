﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RETFIE : Command
    {
        public RETFIE(ushort opcode) : base(opcode)
        {
            Debug.Log("RETFIE");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0000_1001;
        }

        protected override int updateProgramCounter(Memory memory)
        {
            Debug.Log("running RETFIE");
            return 2;
        }
    }
}
