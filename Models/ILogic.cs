using spauldo_tecture.Validation;

namespace spauldo_tecture.Models
{
    public interface ILogic
    {
        Task<SpauldoValidationModel> GetById<TConcreteEntity>(string id)
            where TConcreteEntity : class, IEntity;
        Task<SpauldoValidationModel> GetListByIds<TConcreteEntity>(List<string> ids)
            where TConcreteEntity : class, IEntity;
        Task<SpauldoValidationModel> Save(IDto dto);
        Task<SpauldoValidationModel> SaveList(List<IDto> dto);
        Task<SpauldoValidationModel> RemoveById<TConcreteEntity>(string id)
            where TConcreteEntity : class, IEntity;
        Task<SpauldoValidationModel> RemoveListByIds<TConcreteEntity>(List<string> ids)
            where TConcreteEntity : class, IEntity;
    }
}