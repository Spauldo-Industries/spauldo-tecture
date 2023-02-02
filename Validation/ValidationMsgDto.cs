using HashidsNet;
using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    public class ValidationMsgDto : SpauldoDto
    {
        public string Message { get; set; }
        // TODO message type?

        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

        public override IModel MapToModel(IHashids hashids)
        {
            return new ValidationMsgModel(hashids)
            {
                Id = String.IsNullOrEmpty(this.Id) ? default(int) : hashids.Decode(this.Id).FirstOrDefault(),
                Message = this.Message
            };
        }
    }
}