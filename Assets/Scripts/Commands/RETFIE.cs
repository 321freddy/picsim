using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RETFIE : Command
    {
        public RETFIE(string command) : base(command)
        {
            Debug.Log("RETFIE");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0000_1001;
        }

        public override void run()
        {
            Debug.Log("running RETFIE");
        }
    }
}
