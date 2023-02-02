using spauldo_tecture.Validation;

namespace spauldo_tecture.Models
{
    public interface IModel
    {
        int? Id { get; }
        string CreateBy { get; }
        DateTime CreateDate { get; }
        string  ModifyBy { get; }
        DateTime ModifyDate { get; }
        void AssignId(int id);
        SpauldoEntity MapToEntity();
        SpauldoDto MapToDto();
        SpauldoValidationModel Validate(bool constraining = true);
        string EncodeId();
        string EncodeId(int id);
        int DecodeId(string id);
    }
}