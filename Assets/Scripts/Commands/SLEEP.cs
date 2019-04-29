using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class SLEEP : Command
    {
        public SLEEP(ushort opcode) : base(opcode)
        {
            Debug.Log("SLEEP");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0110_0011;
        }

        public override int run(Memory memory)
        {
            Debug.Log("running SLEEP");
            return base.run(memory); // Increase PC
        }
    }
}
