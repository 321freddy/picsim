using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private Command[] commands;

    public static Program Parse(List<string> lines)
    {
        var commands = new List<Command>();

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line.Substring(0, 8)))
            {
                var command = Command.Parse(line.Substring(5, 4));
                commands.Add(command);
            }
        }

        return new Program
        {
            commands = commands.ToArray(),
        };
    }

}
