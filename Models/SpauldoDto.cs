using HashidsNet;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoDto : IDto
    {
        public virtual string Id { get; set; }
        public abstract IModel MapToModel(IHashids hashids);
    }
}