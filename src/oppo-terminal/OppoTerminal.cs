using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public class OppoTerminal
    {
        private readonly IObjectModel _objectModel;

        public OppoTerminal(IObjectModel objectModel, IWriter writer)
        {
            _objectModel = objectModel;
            writer.WriteLine(Constants.HelloString);
        }

        public string Execute(string[] inputParams)
        {
            System.Console.WriteLine("caveman :P");
            return _objectModel.ExecuteCommand(inputParams);
        }
    }
}