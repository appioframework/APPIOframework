using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class CommandArgumentParser
    {
        private readonly Dictionary<string, string> _flagValuesForShortName;
        private readonly Dictionary<string, string> _flagValuesForVerboseName;

        public CommandArgumentParser(IReadOnlyList<ArgumentInfo> infos, IReadOnlyList<string> arguments)
        {
            _flagValuesForShortName = new Dictionary<string, string>();
            _flagValuesForVerboseName = new Dictionary<string, string>();

            foreach (var info in infos)
            {
                ParseArgumentsByInfos(info, arguments);
            }
        }

        private void ParseArgumentsByInfos(ArgumentInfo info, IReadOnlyList<string> arguments)
        {
            for (var i = 0; i < arguments.Count; ++i)
            {
                var possibleFlag = arguments[i];

                if (info.HasValue)
                {
                    if (i == arguments.Count - 1)
                    {
                        break;
                    }

                    var nextArgument = arguments[i + 1];

                    if (IsCommandLineFlag(possibleFlag, info))
                    {
                        _flagValuesForShortName.Add(info.Name, nextArgument);
                        _flagValuesForVerboseName.Add(info.VerboseName, nextArgument);
                    }
                }
                else
                {
                    if (IsCommandLineFlag(possibleFlag, info))
                    {
                        _flagValuesForShortName.Add(info.Name, string.Empty);
                        _flagValuesForVerboseName.Add(info.VerboseName, string.Empty);
                    }
                }
            }
        }
        
        private static bool IsCommandLineFlag(string possibleFlag, ArgumentInfo info)
        {
            return possibleFlag == info.Name || possibleFlag == info.VerboseName;
        }

        public Argument this[string key]
        {
            get
            {
                if (_flagValuesForShortName.ContainsKey(key))
                {
                    return new Argument
                    {
                        Value = _flagValuesForShortName[key],
                        IsNotExistent = false,
                    };
                }

                if (_flagValuesForVerboseName.ContainsKey(key))
                {
                    return new Argument
                    {
                        Value = _flagValuesForVerboseName[key],
                        IsNotExistent = false,
                    };
                }

                return new Argument
                {
                    Value = string.Empty,
                    IsNotExistent = true,
                };
            }
        }

        public class ArgumentInfo
        {
            public ArgumentInfo(string name, string verboseName, bool hasValue)
            {
                Name = name;
                VerboseName = verboseName;
                HasValue = hasValue;
            }

            public string Name { get; }
            public string VerboseName { get; }
            public bool HasValue { get; }
        }

        public struct Argument
        {
            public string Value { get; set; }
            public bool IsNotExistent { get; set; }
        }
    }
}
