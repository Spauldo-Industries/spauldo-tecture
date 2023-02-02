using HashidsNet;
using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    public class SpauldoValidationModel : SpauldoModel, IValidation
    {
        public bool IsValidityConstraining { get; init; } = true;
        public bool IsValid => Messages.Count == 0;
        public bool IsValidationConstraining => IsValidityConstraining;
        private readonly IHashids _hashids;
        private readonly List<object> _objects = new List<object>();
        private readonly List<ValidationMsgModel> _messages = new List<ValidationMsgModel>();

        public List<object> Objects => _objects;
        public List<ValidationMsgModel> Messages => _messages;
        public string MessagesAsFormattedString => String.Join('\n', Messages.Select(m => m.Message).ToList());

        public override String CreateBy { get; set; }
        public override DateTime CreateDate { get; set; }
        public override String ModifyBy { get; set; }
        public override DateTime ModifyDate { get; set; }

        public SpauldoValidationModel(IHashids hashids) : base(hashids) { _hashids = hashids; }

        public void AddMessage(string msg) => AddMessage(new ValidationMsgModel(_hashids){ Message = msg });
        public void AddMessages(List<string> msgs) => msgs.ForEach(m => AddMessage(m));
        public void AddMessage(ValidationMsgModel msg) => _messages.Add(msg);
        public void AddMessages(List<ValidationMsgModel> msgs) => _messages.AddRange(msgs);
        public void AddObject(object obj) => _objects.Add(obj);
        public void AddObjects(List<object> objs) => _objects.AddRange(objs);
        
        public void Validate()
        {
            if (!IsValid)
                throw new InvalidOperationException(MessagesAsFormattedString);
        }

        public void MergeValidation(SpauldoValidationModel validation)
        {
            AddObjects(validation.Objects);
            AddMessages(validation.Messages);
        }

        public void MergeValidations(List<SpauldoValidationModel> validations)
        {
            validations.ForEach(x => MergeValidation(x));
        }

        public override SpauldoValidationDto MapToDto()
        {
            // if (!this.Id.HasValue) throw new InvalidOperationException();

            return new SpauldoValidationDto
            {
                Id = EncodeId(),
                IsValid = this.IsValid,
                Objects = Objects,
                Messages = Messages.Select(m => m.MapToDto()).ToList(),
            };
        }

        public override SpauldoValidationEntity MapToEntity()
        {
            return new SpauldoValidationEntity 
            {
                ValidationId = Id,
                IsValid = this.IsValid,
                IsConstraining = IsValidityConstraining,
                CreateBy = CreateBy,
                CreateDate = CreateDate,
                ModifyBy = ModifyBy,
                ModifyDate = ModifyDate
            };
        }
    }
}