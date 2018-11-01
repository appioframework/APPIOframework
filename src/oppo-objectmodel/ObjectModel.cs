using System.Collections.Generic;
using System;

namespace Oppo.ObjectModel
{
    public class ObjectModel
    {
        public ObjectModel(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                throw new ArgumentNullException(nameof(inputParams));
            }

            
        }
    }
}