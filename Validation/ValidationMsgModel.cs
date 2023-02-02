using HashidsNet;
using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    public class ValidationMsgModel : SpauldoModel
    {
        public int ValidationId { get; set; }
        public string Message { get; set; }
        // TODO Message type?
        public override string CreateBy { get; set; }
        public override DateTime CreateDate { get; set; }

        public ValidationMsgModel(IHashids hashids) : base(hashids) {  }

        public override ValidationMsgDto MapToDto()
        {
            return new ValidationMsgDto
            {
                Id = EncodeId(),
                Message = this.Message,
                CreateBy = this.CreateBy,
                CreateDate = this.CreateDate,
            };
        }

        public override ValidationMsgEntity MapToEntity()
        {
            return new ValidationMsgEntity
            {
                ValidationMsgId = this.Id,
                ValidationId = this.ValidationId,
                Message = Message
            };
        }

        public override SpauldoValidationModel Validate(bool constraining = false)
        {
            var validation = base.Validate(constraining: constraining);
            return validation;
        }
    }
}