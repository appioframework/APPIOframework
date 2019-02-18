using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class Solution : ISolution
    {        
        public List<Opcuaapp> Projects { get; private set; } = new List<Opcuaapp>();
    }
}