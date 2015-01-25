using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using KidC;
using Engine.SharpDX;

namespace KidCSharpDX
{
    static class Program
    {
        public static void Main()
        {
            var engine = new SharpDXEngine();
            engine.Run(new KidCGame());
        }

    }


}
