using System;
using Engine.XNA;
using KidC;

namespace KidCXNA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var engine = new XNAEngine();
            var game = new KidCGame();

            engine.Run(game);

        }
    }

}

