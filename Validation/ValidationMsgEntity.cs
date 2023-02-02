using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HashidsNet;
using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    [Table("spauldotechture_ValidationMsg")]
    public class ValidationMsgEntity : SpauldoEntity
    {
        [Key]
        [Column("ValidationMsgId")]
        public int? ValidationMsgId { get; set; }
        [Column("ValidationId")]
        public int? ValidationId { get; set; }
        [Column("Message")]
        public string Message { get; set; }
        [Column("CreateBy")]
        public override string CreateBy { get; set; }
        [Column("CreateDate")]
        public override DateTime CreateDate { get; set; }

        public override IModel MapToModel(IHashids hashids)
        {
            return new ValidationMsgModel(hashids)
            {
                Id = this.Id,
                Message = Message,
                CreateBy = CreateBy,
                CreateDate = CreateDate
            };
        }
    }
}