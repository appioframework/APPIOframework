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
            return _objectModel.ExecuteCommand(inputParams);
        }
    }
}