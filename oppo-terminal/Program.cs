using System;

namespace OPPO.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            new OppoTerminal(new ConsoleWriter());            
        }
    }
}