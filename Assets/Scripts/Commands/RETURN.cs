using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    class RETURN : Command
    {
        public RETURN(string command) : base(command)
        {
            Debug.Log("RETURN");
        }

        public static bool check(string command)
        {
            var opcode = Convert.ToInt32(command, 16);
            return (opcode & 0b0011_1111_1111_1111) == 0b0000_0000_0000_1000;
        }

        public override void run()
        {
            Debug.Log("running RETURN");
        }
    }
}
