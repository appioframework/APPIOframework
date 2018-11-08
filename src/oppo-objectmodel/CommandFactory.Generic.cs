using System;
using System.Collections.Generic;
using Oppo.ObjectModel.Exceptions;

namespace Oppo.ObjectModel
{
    public class CommandFactory<TDependance> : ICommandFactory<TDependance>
    {
        private readonly Dictionary<string, ICommand<TDependance>> _commands = new Dictionary<string, ICommand<TDependance>>();
        private readonly string _nameOfDefaultCommand;

        public CommandFactory(IEnumerable<ICommand<TDependance>> commandArray, string nameOfDefaultCommand)
        {
            if (commandArray == null)
            {
                throw new ArgumentNullException(nameof(commandArray));
            }

            _nameOfDefaultCommand = nameOfDefaultCommand ?? throw new ArgumentNullException(nameof(nameOfDefaultCommand));

            foreach (var command in commandArray)
            {
                if (_commands.ContainsKey(command.Name))
                {
                    throw new DuplicateNameException(command.Name);
                }

                _commands.Add(command.Name, command);
            }

            if (_commands.Count > 0 && !_commands.ContainsKey(nameOfDefaultCommand))
            {
                throw new ArgumentOutOfRangeException(nameof(nameOfDefaultCommand));
            }
        }

        public IEnumerable<ICommand<TDependance>> Commands => _commands.Values;

        public ICommand<TDependance> GetCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                return _commands[_nameOfDefaultCommand];
            }

            if (_commands.ContainsKey(commandName))
            {
                return _commands[commandName];
            }

            return new FallbackCommand();
        }

        private class FallbackCommand : ICommand<TDependance>
        {
            public string Name => throw new NotSupportedException();

            public string Execute(IEnumerable<string> inputParams)
            {
                throw new NotSupportedException();
            }

            public string GetHelpText()
            {
                throw new NotSupportedException();
            }
        }
    }
}
