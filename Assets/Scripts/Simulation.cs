using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Simulation
{
    public Command[] Commands;
    public Memory Memory;

    public static Simulation CreateFromProgram(List<string> lines)
    {
        var commands = new List<Command>();
        int lineNum = 0;

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line.Substring(0, 8)))
            {
                var command = Command.Parse(line.Substring(5, 4), lineNum);
                commands.Add(command);
            }

            lineNum++;
        }

        return new Simulation
        {
            Commands = commands.ToArray(),
            Memory = new Memory(),
        };
    }

    public Command getCurrentCommand() // Returns null if program ended
    {
        var pc = Memory.ProgramCounter;

        if (pc < Commands.Length)
        {
            return Commands[pc];
        }

        return null;
    }

    public bool step() // Runs one command and increases PC. Returns false if program ended.
    {
        var cmd = getCurrentCommand();
        if (cmd == null) return false;

        cmd.run(Memory);
        return true;
    }

    public bool reachedBreakpoint()
    {
        var cmd = getCurrentCommand();
        if (cmd == null) return false;

        return cmd.breakpoint;
    }

    public void Reset()
    {
        Memory.Reset();
    }
}
