using spauldo_tecture.Validation;
using HashidsNet;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoModel : IModel
    {
        private readonly IHashids _hashids;
        public int? Id { get; set; }
        public virtual string CreateBy { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string ModifyBy { get; set; }
        public virtual DateTime ModifyDate { get; set; }
        public SpauldoModel(IHashids hashids)
        {
            _hashids = hashids;
        }

        public void AssignId(int id)
        {
            Id = id;
        }

        public virtual SpauldoValidationModel Validate(bool constraining = true)
        {
            SpauldoValidationModel validation = new SpauldoValidationModel(_hashids){ IsValidityConstraining = constraining };
            if (!Id.HasValue)
                validation.AddMessage($"{this.GetType().Name}.Id is null.");
            return validation;
        }

        public string EncodeId()
        {
            this.Validate().Validate();
            return _hashids.Encode(Id.GetValueOrDefault());
        }

        public virtual string EncodeId(int id)
        {
            return _hashids.Encode(id);
        }

        public int DecodeId(string id)
        {
            return _hashids.Decode(id).FirstOrDefault();
        }

        public virtual SpauldoEntity MapToEntity() { throw new NotImplementedException(); }
        public virtual SpauldoDto MapToDto() { throw new NotImplementedException(); }
    }
}