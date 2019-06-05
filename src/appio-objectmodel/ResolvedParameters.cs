using System;

namespace Appio.ObjectModel
{
    public class ResolvedParameters<TIdent> where TIdent : struct, IConvertible
    {
        public string ErrorMessage { get; set; }
        public ParameterList<string, TIdent> StringParameters { get; set; }
        public ParameterList<bool, TIdent> BoolParameters { get; set; }

        public void Deconstruct(out string error, out ParameterList<string, TIdent> stringParams, out ParameterList<bool, TIdent> boolParams)
        {
            error = ErrorMessage;
            stringParams = StringParameters;
            boolParams = BoolParameters;
        }
    }
}