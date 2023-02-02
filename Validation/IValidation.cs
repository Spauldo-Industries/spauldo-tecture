using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    public interface IValidation
    {
        bool IsValid { get; }
        bool IsValidationConstraining { get; }
        List<object> Objects { get; }
        List<ValidationMsgModel> Messages { get; }
        string MessagesAsFormattedString { get; }
        void AddMessage(string msg);
        void AddMessages(List<string> msgs);
        void AddMessage(ValidationMsgModel msg);
        void AddMessages(List<ValidationMsgModel> msgs);
        void AddObject(object obj);
        void AddObjects(List<object> objs);
        void MergeValidation(SpauldoValidationModel validation);
        void MergeValidations(List<SpauldoValidationModel> validations);
        void Validate();
    }
}