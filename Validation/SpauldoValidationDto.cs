using spauldo_tecture.Models;
using HashidsNet;

namespace spauldo_tecture.Validation
{
    public class SpauldoValidationDto : SpauldoDto
    {
        public bool IsValid { get; set; }
        public List<object> Objects { get; set; }
        public List<ValidationMsgDto> Messages { get; set; }

        
        public override IModel MapToModel(IHashids hashids)
        {
            throw new NotImplementedException();
        }
    }
}