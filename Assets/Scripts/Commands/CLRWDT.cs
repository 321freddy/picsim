using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class CLRWDT : Command
    {
        public CLRWDT(ushort opcode, int line) : base(opcode, line)
        {
            Debug.Log("CLRWDT");
        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {

            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0110_0100;
        }

        protected override void runCommand(Memory memory)
        {
            Debug.Log("running CLRWDT");    
            memory.Prescaler = 0;
            memory.Status = (byte) Bit.set(memory.Status, Bit.PD);
            memory.Status = (byte) Bit.set(memory.Status, Bit.TO);
            Timer.ClearWDT();
        }
    }
}
