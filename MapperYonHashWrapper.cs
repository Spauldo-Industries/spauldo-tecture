using HashidsNet;

namespace spauldo_techture;
public abstract class MapperYonHashWrapper<TEntity, TModel, TDto, TAudit> : MapperYonHash<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    protected MapperYonHashWrapper(IHashids hashids) : base(hashids)
    {
    }

    protected override async Task<TEntity> MapDtoToEntity(TDto dto, bool withValidation)
    {
        var mappedModel = await MapDtoToModel(dto, withValidation);
        return await MapModelToEntity(mappedModel, withValidation);
    }

    protected override async Task<TDto> MapEntityToDto(TEntity entity, bool withValidation)
    {
        var mappedModel = await MapEntityToModel(entity, withValidation);
        return await MapModelToDto(mappedModel, withValidation);
    }
}