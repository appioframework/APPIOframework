using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using System;

namespace Oppo.ObjectModel
{
    public class SlnCommandStrategyFactory : ISlnCommandStrategyFactory
    {
        private readonly Dictionary< string, ISlnCommandStrategy > commands
            = new Dictionary< string, ISlnCommandStrategy >();

        public SlnCommandStrategyFactory()
        {
            
        }

        public ISlnCommandStrategy GetStrategy( string commandName )
        {
            // if ( commands.ContainsKey( commandName ?? string.Empty ) )
            // {
            //     return commands[ commandName ];
            // }

            // return new SlnCommandNotExistentStrategy();
            throw new NotImplementedException();
        }
    }
}