using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public interface ISolution
    {
        List<IOpcuaapp> Projects { get; }
    }
}