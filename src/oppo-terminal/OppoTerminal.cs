using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public class OppoTerminal
    {
        private readonly IObjectModel _objectModel;

        public OppoTerminal(IObjectModel objectModel)
        {
            _objectModel = objectModel;
        }

        public string Execute(string[] inputParams)
        {
            return _objectModel.ExecuteCommand(inputParams);
        }
    }
}