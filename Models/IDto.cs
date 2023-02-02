using HashidsNet;

namespace spauldo_tecture.Models
{
    public interface IDto
    {
        string Id { get; }
        IModel MapToModel(IHashids hashids);
    }
}