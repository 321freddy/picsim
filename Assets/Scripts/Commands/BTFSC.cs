using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class BTFSC : Command
    {
        public BTFSC(string command) : base(command)
        {
            Debug.Log("BCF");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1100_0000_0000) == 0b0001_1000_0000_0000;
        }

        public override void run()
        {
            Debug.Log("running BTFSC");
        }
    }
}
