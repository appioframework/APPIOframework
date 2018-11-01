using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using System;

namespace Oppo.ObjectModel
{
    public class SlnCommandStrategyFactory : ISlnCommandStrategyFactory
    {
        private readonly Dictionary< string, ISlnCommandStrategy > slnCommands
            = new Dictionary< string, ISlnCommandStrategy >();

        public SlnCommandStrategyFactory()
        {
            slnCommands.Add(Constants.SlnCommandName.New, new SlnNewCommandStrategy());
        }

        public ISlnCommandStrategy GetStrategy( string slnCommandName )
        {
            if ( slnCommands.ContainsKey( slnCommandName ?? string.Empty ) )
            {
                return slnCommands[ slnCommandName ];
            }

            return new SlnCommandNotExistentStrategy();
        }
    }
}