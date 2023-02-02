using Microsoft.AspNetCore.Mvc;
using spauldo_tecture.Validation;

namespace spauldo_tecture.Models
{
    public interface IController
    {
        Task<IActionResult> Get<TConcreteController, TConcreteEntity>(string id)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity;
        Task<IActionResult> GetList<TConcreteController, TConcreteEntity>(List<string> ids)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity;
        Task<IActionResult> Post<TConcreteController>(IDto dto)
            where TConcreteController : SpauldoController;
        Task<IActionResult> PostList<TConcreteController>(List<IDto> dtos)
            where TConcreteController : SpauldoController;
        Task<IActionResult> Delete<TConcreteController, TConcreteEntity>(string id)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity;
        Task<IActionResult> DeleteList<TConcreteController, TConcreteEntity>(List<string> ids)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity;
    }
}