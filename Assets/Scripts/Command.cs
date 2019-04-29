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

    public virtual int run(Memory memory)
    {
        memory.ProgramCounter++;
        return 1;
    }
}
