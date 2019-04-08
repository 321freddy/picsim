using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class Parser
    {

        private static string[] Parse(string[] lines)
        {
            var program = new List<string>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line.Substring(0, 8)))
                {
                    //var programCounter = int.Parse(line.Substring(0, 4));
                    //var command = line.Substring(5, 4);
                    //program.Add(programCounter, command);
                }
            }

            return program.ToArray();
        }

    }
}
