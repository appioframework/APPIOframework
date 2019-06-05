namespace Appio.ObjectModel
{
    public interface IModelValidator
    {
        bool Validate(string filePathToValidate, string fileNameToValidateAgainst);
    }
}