using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using spauldo_tecture.Validation;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ILogic _logic;
        public SpauldoController
        (
            ILogger<SpauldoController> logger,
            ILogic logic
        )
        {
            _logger = logger;
            _logic = logic;
        }

        public async Task<IActionResult> Get<TConcreteController, TConcreteEntity>(string id)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity
        {
            try
            {
                // TODO - Authentication
                if (String.IsNullOrEmpty(id))
                    return BadRequest();
                if (id.Length < 11 || id.Length > 11)
                    return BadRequest();
                
                var result = await _logic.GetById<TConcreteEntity>(id);

                if (!result.IsValid && !result.IsValidationConstraining)
                {
                    _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET() - Object with id: {id} not found. Error Messages: {result.MessagesAsFormattedString}");
                    return NotFound(result);
                }

                if (!result.IsValid && result.IsValidationConstraining)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET() - Failed to get entityId: {id}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IActionResult> GetList<TConcreteController, TConcreteEntity>(List<string> ids)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity
        {
            try
            {
                // TODO - Authentication
                if (ids == null)
                    return BadRequest();
                if (ids.Any(o => String.IsNullOrEmpty(o)))
                    return BadRequest();
                if (ids.Any(o => o.Length < 11 || o.Length > 11))
                    return BadRequest();
                
                var result = await _logic.GetListByIds<TConcreteEntity>(ids);

                if (!result.IsValid && !result.IsValidationConstraining)
                {
                    _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET LIST() - No objects found. Error Messages: {result.MessagesAsFormattedString}");
                    return NotFound(result);
                }

                if (!result.IsValid && result.IsValidationConstraining)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET LIST() - Failed to get entity list. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - GET LIST() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IActionResult> Post<TConcreteController>(IDto dto)
            where TConcreteController : SpauldoController
        {
            try
            {
                // TODO - Authentication
                if (dto == null)
                    return BadRequest();
                
                var result = await _logic.Save(dto);

                if (!result.IsValid)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST() - Failed to get entity list. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                if (result.Objects.Count == 0 && !result.IsValidationConstraining)
                {
                    _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST() - object with id: {dto.Id} not saved. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IActionResult> PostList<TConcreteController>(List<IDto> dtos)
            where TConcreteController : SpauldoController
        {
            try
            {
                // TODO - Authentication
                if (dtos == null)
                    return BadRequest();
                if (dtos.Any(o => o == null))
                    return BadRequest();
                
                var result = await _logic.SaveList(dtos);

                if (!result.IsValid)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST LIST() - Failed to get entity list. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                if (result.Objects.Count == 0 && !result.IsValidationConstraining)
                {
                    _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST LIST() - objects not saved. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - POST LIST() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<IActionResult> Delete<TConcreteController, TConcreteEntity>(string id)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity
        {
            try
            {
                // TODO - Authentication
                if (String.IsNullOrEmpty(id))
                    return BadRequest();
                if (id.Length < 11 || id.Length > 11)
                    return BadRequest();
                
                var result = await _logic.RemoveById<TConcreteEntity>(id);

                if (!result.IsValid)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE() - Failed to delete id: {id}. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                if (result.Objects.Count == 0 && !result.IsValidationConstraining)
                {
                    _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE() - object with id: {id} not deleted. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IActionResult> DeleteList<TConcreteController, TConcreteEntity>(List<string> ids)
            where TConcreteController : SpauldoController
            where TConcreteEntity : class, IEntity
        {
            try
            {
                // TODO - Authentication
                if (ids == null)
                    return BadRequest();
                if (ids.Any(o => String.IsNullOrEmpty(o)))
                    return BadRequest();
                if (ids.Any(o => o.Length < 11 || o.Length > 11))
                    return BadRequest();
                
                var result = await _logic.RemoveListByIds<TConcreteEntity>(ids);

                if (!result.IsValid)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE LIST() - Failed to get entity list. Error Messages: {result.MessagesAsFormattedString}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                if (result.Objects.Count == 0 && !result.IsValidationConstraining)
                {
                    _logger.LogError($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE LIST() - No objects deleted. Error Messages: {result.MessagesAsFormattedString}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(result.MapToDto());
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{DateTime.Now}: {this.GetType().Name : T.GetType().Name} - DELETE LIST() - Failed with exception {e.Message}, {e.StackTrace}.");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}