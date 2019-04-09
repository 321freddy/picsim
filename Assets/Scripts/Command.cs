using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

abstract class Command
{
    public static Command Parse(string command)
    {
        var args = new object[] { command };
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
                return (Command) commandType.GetConstructors()[0].Invoke(args);
            }
        }

        throw new Exception("Unknwon cmd: " + command);
    }

    public Command(string command)
    {
        Debug.Log(command[0]);
        Debug.Log("new cmd " + command);
        Memory.p_Counter++;
    }

    public abstract void run();
}
