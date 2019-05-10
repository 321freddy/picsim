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
        public SLEEP(ushort opcode, int line) : base(opcode, line)
        {

        }

        public static bool check(ushort opcode) // Return true if opcode contains this command
        {
            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0110_0011;
        }

        protected override void checkForInterrupt(Memory memory)
        {
            memory.runRBEdgeDetection();

            // Ignore GIE and TMR0 Interrupt

            if (Bit.get(memory.INTCON, Bit.INTE) == 1 && // RB0/INT interrupt
                Bit.get(memory.INTCON, Bit.INTF) == 1) 
            {
                Debug.Log("INTERRUPT RB0/INT");
                fireInterrupt(memory);
            }
            else 
            if (Bit.get(memory.INTCON, Bit.RBIE) == 1 && // RB interrupt
                Bit.get(memory.INTCON, Bit.RBIF) == 1) 
            {
                Debug.Log("INTERRUPT RB");
                fireInterrupt(memory);
            }
        }
    
        protected override void fireInterrupt(Memory memory)
        {
            memory.Sleeping = false;
        }

        public override int run(Memory memory)
        {
            if (!memory.Sleeping)  // first sleep call
            {
                memory.Prescaler = 0;
                memory.Status = (byte) Bit.clear(memory.Status, Bit.PD);
                memory.Status = (byte) Bit.set(memory.Status, Bit.TO);
            }

            memory.Sleeping = true;

            updateTimer(memory);
            updateWatchdog(memory);
            checkForInterrupt(memory);

            if (!memory.Sleeping) memory.ProgramCounter++;
            return 1;
        }
    }
}
