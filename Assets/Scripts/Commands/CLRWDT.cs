using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class CLRWDT : Command
    {
        public CLRWDT(string command) : base(command)
        {
            Debug.Log("CLRWDT");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0110_0100;
        }

        public override void run()
        {
            Debug.Log("running CLRWDT");
        }
    }
}
