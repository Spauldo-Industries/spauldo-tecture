using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HashidsNet;
using spauldo_tecture.Models;

namespace spauldo_tecture.Validation
{
    [Table("spauldotechture_Validation")]
    public class SpauldoValidationEntity : SpauldoEntity
    {
        [Key]
        [Column("ValidationId")]
        public int? ValidationId { get; set; }

        [Column("IsValid")]
        public bool IsValid { get; set; }

        [Column("IsConstraining")]
        public bool IsConstraining { get; set; }


        [Column("CreateBy")]
        public override String CreateBy { get; set; }
        [Column("CreateDate")]
        public override DateTime CreateDate { get; set; }

        public override IModel MapToModel(IHashids hashids)
        {
            return new SpauldoValidationModel(hashids)
            {
                Id = this.Id,
                IsValidityConstraining = this.IsConstraining
            };
        }
    }
}