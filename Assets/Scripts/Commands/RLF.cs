using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RLF : Command
    {
        public RLF(string command) : base(command)
        {
            Debug.Log("RLF");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_0000_0000) == 0b00_1101_0000_0000;
        }

        public override void run()
        {
            Debug.Log("running RLF");
        }
    }
}
