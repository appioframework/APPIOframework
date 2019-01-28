namespace Oppo.ObjectModel
{
    public interface IModelValidator
    {
        bool Validate(string filePathToValidate, string fileNameToValidateAgainst);
    }
}