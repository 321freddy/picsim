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

    // Runs one command and increases PC. Returns amount of cycles executed. (0 if program ended)
    public int step() => getCurrentCommand()?.run(Memory) ?? 0;

    public bool reachedBreakpoint() => getCurrentCommand()?.breakpoint ?? false;

    public void Reset() => Memory.Reset();
}
