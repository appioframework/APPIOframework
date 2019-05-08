namespace Oppo.ObjectModel
{
    public abstract class ParameterSpecification<TIdent>
    {
        public TIdent Identifier { get; set; }
        public string Short { get;  set; }
        public string Verbose { get;  set; }

        public void Deconstruct(out TIdent identifier, out string shortName, out string verboseName)
        {
            identifier = Identifier;
            shortName = Short;
            verboseName = Verbose;
        }
    }
    
    public class StringParameterSpecification<TIdent> : ParameterSpecification<TIdent>
    {
        public string Default { get; set; }
    }
    
    public class BoolParameterSpecification<TIdent> : ParameterSpecification<TIdent>
    {
    }
}