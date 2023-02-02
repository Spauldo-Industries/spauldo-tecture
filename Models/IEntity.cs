using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HashidsNet;

namespace spauldo_tecture.Models
{
    public interface IEntity
    {
        [NotMapped]
        int Id { get; }
        string CreateBy { get; }
        DateTime CreateDate { get; }
        string  ModifyBy { get; }
        DateTime ModifyDate { get; }
        public IModel MapToModel(IHashids hashids);
    }
}