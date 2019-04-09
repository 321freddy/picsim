using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class NOP : Command
    {
        public NOP(string command) : base(command)
        {
            Debug.Log("NOP");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_1001_1111) == 0;
        }

        public override void run()
        {
            Debug.Log("running NOP");
        }
    }
}
