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

    public static Command Parse(string command, int line, int internalLine)
    {
        var checkArgs  = new object[] { Convert.ToUInt16(command, 16) };
        var constrArgs = new object[] { checkArgs[0], internalLine };

        var types = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.Namespace?.Equals("Commands") == true);

        // Iterate over all Commands
        foreach (var commandType in types)
        {
            // Check if the mask and OpCode matches the command
            if ((bool) commandType.GetMethod("check").Invoke(null, checkArgs))
            {
                // Mask and OpCode matches...
                var cmd = (Command) commandType.GetConstructors()[0].Invoke(constrArgs);
                cmd.Line = line;
                return cmd;
            }
        }

        throw new Exception("Unbekannter Befehl: " + command);
    }

    public Command(ushort opcode, int line)
    {
        
    }

    protected virtual void runCommand(Memory memory)
    {
        // Implemented by subclasses
    }

    protected virtual int updateProgramCounter(Memory memory) // Returns amount of cycles it took to run cmd
    {
        memory.ProgramCounter++;
        return 1;
    }

    protected virtual void updateTimer(Memory memory)
    {
        int fosc4 = 1;
        int addToTMR0 = 0;

        if (Bit.get(memory.OPTION, Bit.T0CS) == 0) // T0CS MUX
        {
            addToTMR0 = fosc4; // add Fosc/4 to TMR0
        }
        else if (memory.runT0CKIEdgeDetection())
        {
            addToTMR0 = 1; // RA4 / T0CKI edge
        }

        if (Bit.get(memory.OPTION, Bit.PSA) == 0) // TMR0 with prescaler?
        {
            memory.Prescaler += (byte) addToTMR0; // add to prescaler register

            // Divide by taking the bit of prescaler at position [PS2:PS0 + 1]
            int bit = memory.PS + 1;
            addToTMR0 = Bit.get(memory.Prescaler, bit, 8);

            if (addToTMR0 > 0) // Reset prescaler if bit is 1
            {
                memory.Prescaler = (byte) Bit.mask(memory.Prescaler, bit); 
            }
        }

        int result = (int) memory.TMR0 + addToTMR0;
        if (result > 0xFF)
        {
            memory.INTCON = (byte) Bit.set(memory.INTCON, Bit.T0IF);
        }

        memory.TMR0 = (byte) result;
    }

    protected virtual void updateWatchdog(Memory memory)
    {
        // WATCHDOG
        bool triggerWatchdog = Timer.TriggerWatchdog;
        // Debug.Log("update wdt: trigger="+triggerWatchdog);

        if (Bit.get(memory.OPTION, Bit.PSA) == 1) // Watchdog with prescaler?
        {
            // Debug.Log("prescaling wdt");
            memory.Prescaler += triggerWatchdog ? (byte)1 : (byte)0; // add to prescaler register

            // Divide by taking the bit of prescaler at position [PS2:PS0 + 1]
            int bit = memory.PS; // +1
            triggerWatchdog = Bit.get(memory.Prescaler, bit, 8) > 0;

            if (triggerWatchdog) // Reset prescaler if bit is 1
            {
                memory.Prescaler = (byte) Bit.mask(memory.Prescaler, bit); 
            }
        }

        // Debug.Log("after wdt: trigger="+triggerWatchdog);
        if (triggerWatchdog) // trigger reset
        {
            if (!memory.Sleeping) memory.Reset();
            memory.Sleeping = false;
            memory.Status = (byte) Bit.clear(memory.Status, Bit.TO);
        }
    }

    protected virtual void checkForInterrupt(Memory memory)
    {
        memory.runRBEdgeDetection();

        if (Bit.get(memory.INTCON, Bit.GIE) == 1) // global interrupt enabled?
        {
            if (Bit.get(memory.INTCON, Bit.T0IE) == 1 && // T0 interrupt
                Bit.get(memory.INTCON, Bit.T0IF) == 1) 
            {
                Debug.Log("INTERRUPT Timer0 overflow");
                fireInterrupt(memory);
            }
            else 
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
            else 
            if (Bit.get(memory.INTCON, Bit.EEIE) == 1 && // EEPROM interrupt
                Bit.get(memory[Address.EECON1], Bit.EEIF) == 1) 
            {
                Debug.Log("INTERRUPT EEPROM");
                fireInterrupt(memory);
            }
        }
    }
    
    protected virtual void fireInterrupt(Memory memory)
    {
        memory.pushStack((byte) (memory.ProgramCounter + 1));
        memory.ProgramCounter = 4;
        memory.INTCON = (byte) Bit.clear(memory.INTCON, Bit.GIE);
    }

    public virtual int run(Memory memory)
    {
        runCommand(memory);
        var cycles = updateProgramCounter(memory);

        // Update timer and run additional NOP's if command takes more than 1 cycle
        for (int i = 0; i < cycles; i++)
        {
            updateTimer(memory);
            updateWatchdog(memory);
            checkForInterrupt(memory);
        }

        return cycles;
    }
}
