using System;

namespace OPPO.Terminal
{
    static class Program
    {
        static void Main(string[] args)
        {
            new OppoTerminal(new ConsoleWriter());            
        }
    }
}