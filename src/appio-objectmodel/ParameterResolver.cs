using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel
{
    public class ParameterResolver<TIdent> where TIdent : struct, IConvertible
    {
        private readonly List<IdentifierNames> _flags;
        private readonly List<DefaultParamValues> _defaultParams;
        private readonly int _numStringParams, _numBoolParams, _boolOffset;
        private readonly string _commandName;

        private struct IdentifierNames
        {
            internal enum DataType { String, Bool }
            public readonly string Name;
            public readonly TIdent Identifier;
            public readonly DataType Type;

            public IdentifierNames(string flagName, TIdent identifier, DataType paramType)
            {
                Name = flagName;
                Identifier = identifier;
                Type = paramType;
            }
        }

        private struct DefaultParamValues
        {
            public readonly TIdent Identifier;
            public readonly string Default;

            public DefaultParamValues(TIdent identifier, string defaultValue)
            {
                Identifier = identifier;
                Default = defaultValue;
            }
        }

        public ParameterResolver(
            string commandName,
            IReadOnlyCollection<StringParameterSpecification<TIdent>> stringParamSpecs,
            IReadOnlyCollection<BoolParameterSpecification<TIdent>> boolParamSpecs = null)
        {
            Debug.Assert(typeof(TIdent).IsEnum, "Use enum type as parameter identifier");

            _commandName = commandName;
            _numStringParams = stringParamSpecs.Count;
            _numBoolParams = boolParamSpecs?.Count ?? 0;
            _flags = new List<IdentifierNames>(capacity: 2 * _numStringParams);
            _defaultParams = new List<DefaultParamValues>();
            _boolOffset = int.MaxValue;

            foreach (var spec in stringParamSpecs)
            {
                RegisterFlagName(spec, IdentifierNames.DataType.String);
                
                if (spec.Default != null)
                    _defaultParams.Add(new DefaultParamValues(spec.Identifier, spec.Default));
            }

            if (boolParamSpecs != null)
            {
                foreach (var spec in boolParamSpecs)
                {
                    RegisterFlagName(spec, IdentifierNames.DataType.Bool);
                    
                    var index = (int) (object) spec.Identifier;
                    if (index < _boolOffset)
                        _boolOffset = index;
                }
            }
        }

        private void RegisterFlagName(ParameterSpecification<TIdent> spec, IdentifierNames.DataType type)
        {
            var (identifier, shortName, verboseName) = spec;
            if (shortName != null)
                _flags.Add(new IdentifierNames(shortName, identifier, type));
            if (verboseName != null)
                _flags.Add(new IdentifierNames(verboseName, identifier, type));
        }

        public ResolvedParameters<TIdent> ResolveParams(IEnumerable<string> parameters)
        {
            var resolvedStringParams = new string[_numStringParams];
            var resolvedBoolParams = new bool[_numBoolParams];
            Array.Fill(resolvedBoolParams, false);
            
            var handledFlags = new List<Tuple<TIdent, string>>();

            var parameterArray = parameters as string[] ?? parameters.ToArray();
            for (var providedFlagNameIndex = 0; providedFlagNameIndex < parameterArray.Length; providedFlagNameIndex++)
            {
                var providedFlagName = parameterArray[providedFlagNameIndex];
                var flagIndex = _flags.FindIndex(pair => pair.Name == providedFlagName);
                if (flagIndex == -1)
                {
                    AppioLogger.Warn(string.Format(LoggingText.UnknownParameterProvided, _commandName));
                    return new ResolvedParameters<TIdent> {ErrorMessage =
                        string.Format(OutputText.UnknownParameterProvided, providedFlagName, JoinFlagString())};
                }

                switch (_flags[flagIndex].Type)
                {
                    case IdentifierNames.DataType.String:
                        ++providedFlagNameIndex;
                        if (providedFlagNameIndex >= parameterArray.Length ||
                            parameterArray[providedFlagNameIndex] == string.Empty)
                        {
                            AppioLogger.Warn(string.Format(LoggingText.ParameterValueMissing, _commandName));
                            return new ResolvedParameters<TIdent>{ErrorMessage = string.Format(OutputText.ParameterValueMissing, _flags[flagIndex].Name)};
                        }
                        resolvedStringParams[(int) (object) _flags[flagIndex].Identifier] = parameterArray[providedFlagNameIndex];
                        break;
                    case IdentifierNames.DataType.Bool:
                        resolvedBoolParams[(int) (object) _flags[flagIndex].Identifier - _boolOffset] = true;
                        break;
                }

                handledFlags.Add(new Tuple<TIdent, string>(_flags[flagIndex].Identifier, providedFlagName));
            }

            if (ContainsDuplicateParameters(handledFlags, out var affectedFlag))
            {
                AppioLogger.Warn(string.Format(LoggingText.DuplicateParameterProvided, _commandName));
                return new ResolvedParameters<TIdent> {ErrorMessage =
                    string.Format(OutputText.DuplicateParameterProvided, affectedFlag)};
            }

            if (!FillDefaultValues(resolvedStringParams, out var missingParamString))
            {
                AppioLogger.Warn(string.Format(LoggingText.MissingRequiredParameter, _commandName));
                return new ResolvedParameters<TIdent>{ErrorMessage =
                    string.Format(OutputText.MissingRequiredParameter, missingParamString)};
            }
                
            return new ResolvedParameters<TIdent>
            {
                StringParameters = new ParameterList<string, TIdent>(0, resolvedStringParams),
                BoolParameters = new ParameterList<bool, TIdent>(_boolOffset, resolvedBoolParams) 
            };
        }

        private bool FillDefaultValues(string[] resolvedStringParams, out string missingParamString)
        {
            for (var i = 0; i < resolvedStringParams.Length; i++)
            {
                if (resolvedStringParams[i] == null)
                {
                    var defaultParamValue = _defaultParams.Find(pair => (int) (object) pair.Identifier == i).Default;

                    if (defaultParamValue == null)
                    {
                        var missingParamNames = (from flag in _flags
                            where (int) (object) flag.Identifier == i
                            select flag.Name).ToArray();
                        var missingParamStringBuilder = new StringBuilder($"'{missingParamNames[0]}'");
                        if (missingParamNames.Length >= 2)
                            missingParamStringBuilder.Append($" or '{missingParamNames[1]}'");

                        missingParamString = missingParamStringBuilder.ToString();
                        return false;
                    }

                    resolvedStringParams[i] = defaultParamValue;
                }
            }

            missingParamString = null;
            return true;
        }

        private static bool ContainsDuplicateParameters(IReadOnlyList<Tuple<TIdent, string>> handledFlags, out string affectedFlag)
        {
            for (var i = 0; i < handledFlags.Count; i++)
            {
                for (var j = 0; j < handledFlags.Count; j++)
                {
                    if (i == j)
                        continue;

                    var (firstIdentifier, _) = handledFlags[i];
                    var (secondIdentifier, duplicateFlag) = handledFlags[j];
                    affectedFlag = duplicateFlag;

                    if ((int) (object) firstIdentifier == (int) (object) secondIdentifier)
                        return true;
                }
            }

            affectedFlag = null;
            return false;
        }

        private string JoinFlagString()
        {
            var joinedFlags = new StringBuilder();

            foreach (var validFlag in _flags)
                joinedFlags.Append($"'{validFlag.Name}', ");

            // Remove redundant comma
            joinedFlags.Length -= 2;

            // Replace last comma with `or`
            var joinedFlagsStr = joinedFlags.ToString();
            var lastComma = joinedFlagsStr.LastIndexOf(",", StringComparison.Ordinal);
            joinedFlagsStr = joinedFlagsStr.Remove(lastComma, 1).Insert(lastComma, " or");
            return joinedFlagsStr;
        }
    }

    public static class ParameterResolver
    {
        public static CommandResult DelegateToSubcommands<T>(ICommandFactory<T> factory, IEnumerable<string> parameters)
        {
            var paramArray = parameters as string[] ?? parameters.ToArray();
            return factory.GetCommand(paramArray.FirstOrDefault()).Execute(paramArray.Skip(1).ToArray());
        }
    }
}