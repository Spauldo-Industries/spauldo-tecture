using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spauldo_techture;
public interface IEntity
{
    int Id { get; }
    string CreateBy { get; }
    DateTime CreateDate { get; }
    string ModifyBy { get; }
    DateTime ModifyDate { get; }
}

public abstract class Entity : IEntity
{
    public abstract int Id { get; }
    [Required]
    [Column("CreateBy")]
    public virtual string CreateBy { get; set; }
    [Required]
    [Column("CreateDate")]
    public virtual DateTime CreateDate { get; set; }
    [Required]
    [Column("ModifyBy")]
    public virtual string ModifyBy { get; set; }
    [Required]
    [Column("ModifyDate")]
    public virtual DateTime ModifyDate { get; set; }
}