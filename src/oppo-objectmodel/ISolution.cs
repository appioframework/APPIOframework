using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface ISolution
    {
        List<IOpcuaapp> Projects { get; }
    }
}