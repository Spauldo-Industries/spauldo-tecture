using Microsoft.AspNetCore.Mvc;
using spauldo_tecture.Validation;
using HashidsNet;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoLogic : ILogic
    {
        private readonly IHashids _hashids;
        private readonly IRepo _repo;

        public SpauldoLogic
        (
            IHashids hashids,
            [FromServices]IRepo repo
        )
        {
            _hashids = hashids;
            _repo = repo;
        }

        public async Task<SpauldoValidationModel> GetById<TConcreteEntity>(string id)
            where TConcreteEntity : class, IEntity
        {
            IEntity entity = await _repo.Select<TConcreteEntity>(_hashids.Decode(id).FirstOrDefault());
            if (entity == null)
            {
                var v = new SpauldoValidationModel(_hashids){ IsValidityConstraining = false };
                v.AddMessage($"{id} not found.");
                return v;
            }
            IModel model = entity.MapToModel(_hashids);
            SpauldoValidationModel validation = model.Validate();
            if (!validation.IsValid && validation.IsValidationConstraining)
                return validation;
            validation.AddObject(model.MapToDto());
            validation.AssignId((await _repo.Insert(validation.MapToEntity())).GetValueOrDefault());
            foreach (var msg in validation.Messages)
            {
                msg.ValidationId = validation.Id.GetValueOrDefault();
                msg.AssignId((await _repo.Insert(msg.MapToEntity())).GetValueOrDefault());
            }
            return validation;
        }

        public async Task<SpauldoValidationModel> GetListByIds<TConcreteEntity>(List<string> ids)
            where TConcreteEntity : class, IEntity
        {
            SpauldoValidationModel validation = await GetById<TConcreteEntity>(ids.FirstOrDefault());
            foreach (string id in ids.Skip(1))
                validation.MergeValidation(await GetById<TConcreteEntity>(id));
            return validation;
        }

        public async Task<SpauldoValidationModel> Save(IDto dto)
        {
            IModel model = dto.MapToModel(_hashids);
            if (model.Id == 0)
                return await Insert(model);
            return await Update(model);
        }

        public async Task<SpauldoValidationModel> SaveList(List<IDto> dtos)
        {
            SpauldoValidationModel validation = await Save(dtos.FirstOrDefault());
            foreach (IDto dto in dtos.Skip(1))
                validation.MergeValidation(await Save(dto));
            return validation;
        }

        private async Task<SpauldoValidationModel> Insert(IModel model)
        {
            SpauldoValidationModel validation = model.Validate();
            if (!validation.IsValid && validation.IsValidationConstraining)
                return validation;
            model.AssignId((await _repo.Insert(model.MapToEntity())).GetValueOrDefault());
            validation.AddObject(model.MapToDto());
            validation.AssignId((await _repo.Insert(validation.MapToEntity())).GetValueOrDefault());
            foreach (var msg in validation.Messages)
            {
                msg.ValidationId = validation.Id.GetValueOrDefault();
                msg.AssignId((await _repo.Insert(msg.MapToEntity())).GetValueOrDefault());
            }
            return validation;
        }

        private async Task<SpauldoValidationModel> Update(IModel model)
        {
            SpauldoValidationModel validation = model.Validate();
            if (!validation.IsValid && validation.IsValidationConstraining)
                return validation;
            model.AssignId(await _repo.Update(model.MapToEntity()));
            validation.AddObject(model.MapToDto());
            validation.AssignId((await _repo.Insert(validation.MapToEntity())).GetValueOrDefault());
            foreach (var msg in validation.Messages)
            {
                msg.ValidationId = validation.Id.GetValueOrDefault();
                msg.AssignId((await _repo.Insert(msg.MapToEntity())).GetValueOrDefault());
            }
            return validation;
        }

        public async Task<SpauldoValidationModel> RemoveById<TConcreteEntity>(string id)
            where TConcreteEntity : class, IEntity
        {
            SpauldoValidationModel validation = new SpauldoValidationModel(_hashids);
            int removedId = await _repo.Delete<TConcreteEntity>(_hashids.Decode(id).FirstOrDefault());
            validation.AddObject(_hashids.Encode(removedId));
            validation.AssignId((await _repo.Insert(validation.MapToEntity())).GetValueOrDefault());
            foreach (var msg in validation.Messages)
            {
                msg.ValidationId = validation.Id.GetValueOrDefault();
                msg.AssignId((await _repo.Insert(msg.MapToEntity())).GetValueOrDefault());
            }
            return validation;
        }

        public async Task<SpauldoValidationModel> RemoveListByIds<TConcreteEntity>(List<string> ids)
            where TConcreteEntity : class, IEntity
        {
            SpauldoValidationModel validation = new SpauldoValidationModel(_hashids);
            foreach (string id in ids)
                validation.MergeValidation(await RemoveById<TConcreteEntity>(id));
            return validation;
        }
    }
}