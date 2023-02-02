using HashidsNet;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoDto : IDto
    {
        public virtual string Id { get; set; }
        public virtual int? Decode(IHashids hashids)
        {
            return String.IsNullOrEmpty(this.Id) ? default(int) : hashids.Decode(this.Id).FirstOrDefault();
        }
        public abstract IModel MapToModel(IHashids hashids);
    }
}