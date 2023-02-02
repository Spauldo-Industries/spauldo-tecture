using HashidsNet;

namespace spauldo_tecture.Models
{
    public interface IDto
    {
        string Id { get; }
        int? Decode(IHashids hashids);
        IModel MapToModel(IHashids hashids);
    }
}