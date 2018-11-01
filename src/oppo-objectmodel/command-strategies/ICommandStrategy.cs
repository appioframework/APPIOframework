using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface ICommandStrategy
    {
        string Execute(IEnumerable<string> inputsArgs);
    }
}