using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HashidsNet;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoEntity : IEntity
    {
        [NotMapped]
        public virtual int Id { get; set; }
        [NotMapped]
        public virtual string CreateBy { get; set; }
        [NotMapped]
        public virtual DateTime CreateDate { get; set; }
        [NotMapped]
        public virtual string ModifyBy { get; set; }
        [NotMapped]
        public virtual DateTime ModifyDate { get; set; }

        public abstract IModel MapToModel(IHashids hashids);
    }
}