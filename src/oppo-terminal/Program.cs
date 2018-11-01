using System;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main(string[] args)
        {
            new OppoTerminal(new ConsoleWriter(), args);            
        }
    }
}