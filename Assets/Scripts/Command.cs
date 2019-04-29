using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Command
{
    public int Line { get; private set; } // Line number in original source code
    public bool breakpoint = false;

    public static Command Parse(string command, int line)
    {
        var args = new object[] { Convert.ToUInt16(command, 16) };
        var types = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.Namespace?.Equals("Commands") == true);

        // Iterate over all Commands
        foreach (var commandType in types)
        {
            // Check if the mask and OpCode matches the command
            if ((bool) commandType.GetMethod("check").Invoke(null, args))
            {

                // Mask and OpCode matches...
                var cmd = (Command) commandType.GetConstructors()[0].Invoke(args);
                cmd.Line = line;
                return cmd;
            }
        }

        throw new Exception("Unbekannter Befehl: " + command);
    }

    public Command(ushort opcode)
    {
        
    }

    protected void updateTimer(Memory memory)
    {
        
        int fosc4 = 1;
        int addToTMR0 = 0;

        if (Bit.get(memory.OPTION, Bit.T0CS) == 0) // T0CS MUX
        {
            addToTMR0 = fosc4;
        }
        else
        {
            // RA4 / T0CKI
            if (Bit.get(memory.OPTION, Bit.T0SE) == 0) // rising edge 0->1
            {

            }
            else // falling edge 1->0
            {

            }
        }

        if (Bit.get(memory.OPTION, Bit.PSA) == 0) // with prescaler?
        {
            memory.Prescaler += (ushort) addToTMR0; // add to prescaler register

            int bit = memory.PS + 1; // apply prescaler by taking the bit denoted by PS2:PS0
            addToTMR0 = Bit.get(memory.Prescaler, bit);
        }

        int result = memory.TMR0 + addToTMR0;
        if (result > 0xFF)
        {
            Bit.set(memory.INTCON, Bit.T0IF);
        }

        memory.TMR0 = (byte) result;
    }

    public virtual int run(Memory memory)
    {
        // Update Timer0...
        updateTimer(memory);

        // Fire interrupt...

        return 1;
    }
}
