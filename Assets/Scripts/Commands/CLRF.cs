using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class CLRF : Command
    {
        public CLRF(string command) : base(command)
        {
            Debug.Log("CLRF");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_1000_0000) == 0b00_0001_1000_0000;
        }

        public override void run()
        {
            Debug.Log("running CLRF");
        }
    }
}
