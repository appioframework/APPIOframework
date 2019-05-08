namespace Oppo.ObjectModel
{
    public class ParameterList<TData, TIdent>
    {
        private readonly int _offset;
        private readonly TData[] _parameters;

        public ParameterList(int offset, TData[] parameters)
        {
            _offset = offset;
            _parameters = parameters;
        }

        public TData this[TIdent identifier] => _parameters[(int) (object) identifier - _offset];
    }
}