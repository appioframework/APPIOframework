using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using Oppo.Resources.text.output;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    class ReferenceAddCommandStrategySholud
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-S", "testServer", "-C", "testProj" },
                new [] { "-S", "testSln", "--client", "testProj" },
                new [] { "--server", "testSln", "-C", "testProj" },
                new [] { "--server", "testSln", "--client", "testProj" },
            };
        }
        protected static string[][] InvalidInputs_UnknownClientParam()
        {
            return new[]
            {
                new [] { "-S", "testSln", "-C", "testProj" },
                new [] { "-S", "testSln", "--client", "testProj" },
                new [] { "--server", "testSln", "-C", "testProj" },
                new [] { "--server", "testSln", "--client", "testProj" },
            };
        }
    }
}
