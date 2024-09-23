using HashidsNet;

namespace spauldo_techture;
/// <summary>
/// Provides a base class for mapping between entity, model, dto, and audit types.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TDto">The dto type.</typeparam>
/// <typeparam name="TAudit">The corresponding audit type.</typeparam>
public abstract class MapperYonHash<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    private readonly IHashids _hashids;
    private readonly Dictionary<Type, List<Func<object, bool, Task<object>>>> _mappers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MapperYonHash{TEntity, TModel, TDto, TAudit}"/> class.
    /// </summary>
    /// <param name="hashids">The Hashids instance for encoding and decoding IDs.</param>
    protected MapperYonHash(IHashids hashids)
    {
        _hashids = hashids;
        InitializeMappers();
    }

    private void InitializeMappers()
    {
        AddMapper(typeof(TModel), async (source, withValidation) => await MapDtoToModel((TDto)source, withValidation));
        AddMapper(typeof(TEntity), async (source, withValidation) => await MapDtoToEntity((TDto)source, withValidation));
        AddMapper(typeof(TDto), async (source, withValidation) => await MapModelToDto((TModel)source, withValidation));
        AddMapper(typeof(TEntity), async (source, withValidation) => await MapModelToEntity((TModel)source, withValidation));
        AddMapper(typeof(TModel), async (source, withValidation) => await MapEntityToModel((TEntity)source, withValidation));
        AddMapper(typeof(TAudit), async (source, withValidation) => await MapEntityToAudit((TEntity)source, withValidation));
        AddMapper(typeof(TDto), async (source, withValidation) => await MapEntityToDto((TEntity)source, withValidation));
    }

    private void AddMapper(Type targetType, Func<object, bool, Task<object>> mapper)
    {
        if (!_mappers.TryGetValue(targetType, out var mapperList))
        {
            mapperList = [];
            _mappers[targetType] = mapperList;
        }

        mapperList.Add(mapper);
    }

    /// <summary>
    /// Maps a collection of objects to the specified target type.
    /// </summary>
    /// <typeparam name="TTarget">The target type to map to.</typeparam>
    /// <param name="source">The source collection to map.</param>
    /// <returns>The mapped collection of the target type.</returns>
    public async Task<List<TTarget>> MapList<TTarget>(IEnumerable<object> source, bool withValidation = false)
    {
        if (source == null)
            throw new NullReferenceException($"Attempted to list map a null source.");
        if (!source.Any())
            return [];

        List<TTarget> targetResults = [];
        foreach (var item in source)
        {
            var result = await Map<TTarget>(item, withValidation);
            targetResults.Add(result);
        }

        return targetResults;
    }

    /// <summary>
    /// Maps an object to the specified target type.
    /// </summary>
    /// <typeparam name="TTarget">The target type to map to.</typeparam>
    /// <param name="source">The source object to map.</param>
    /// <returns>The mapped object of the target type.</returns>
    public async Task<TTarget> Map<TTarget>(object source, bool withValidation = false)
    {
        if (source == null)
            throw new NullReferenceException($"Attempting to map a null source.");

        if (_mappers.TryGetValue(typeof(TTarget), out var mappers))
        {
            foreach (var mapper in mappers)
            {
                var result = await TryMap(mapper, source, withValidation);

                if (result != null)
                {
                    return (TTarget)result;
                }
            }
        }

        throw new NotSupportedException($"Mapping from {source.GetType().Name} to {typeof(TTarget).Name} is not supported.");
    }

    public MapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget> Build<TTarget>(object source)
        where TTarget : class
    {
        return new MapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget>(this, source);
    }

    private static async Task<object> TryMap(Func<object, bool, Task<object>> mapper, object source, bool withValidation)
    {
        try
        {
            return await mapper(source, withValidation);
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Maps an entity to a model.
    /// </summary>
    /// <param name="entity">The entity to map.</param>
    /// <returns>Result object containing the mapped model.</returns>
    protected abstract Task<TModel> MapEntityToModel(TEntity entity, bool withValidation);

    /// <summary>
    /// Maps a model to a dto.
    /// </summary>
    /// <param name="model">The model to map.</param>
    /// <returns>Result object containing the mapped dto.</returns>
    protected abstract Task<TDto> MapModelToDto(TModel model, bool withValidation);

    /// <summary>
    /// Maps a dto to a model.
    /// </summary>
    /// <param name="dto">The dto to map.</param>
    /// <returns>Result object containing the mapped model.</returns>
    protected abstract Task<TModel> MapDtoToModel(TDto dto, bool withValidation);

    /// <summary>
    /// Maps a model to an entity.
    /// </summary>
    /// <param name="model">The model to map.</param>
    /// <returns>Result object containing the mapped entity.</returns>
    protected abstract Task<TEntity> MapModelToEntity(TModel model, bool withValidation);

    /// <summary>
    /// Maps a entity to an audit entity.
    /// </summary>
    /// <param name="entity">The entity to map.</param>
    /// <returns>Result object containing the mapped audit entity.</returns>
    protected abstract Task<TAudit> MapEntityToAudit(TEntity entity, bool withValidation);

    /// <summary>
    /// Maps a dto to a entity.
    /// </summary>
    /// <param name="dto">The dto to map.</param>
    /// <returns>Result object containing the mapped entity.</returns>
    protected abstract Task<TEntity> MapDtoToEntity(TDto dto, bool withValidation);

    /// <summary>
    /// Maps an entity to a dto.
    /// </summary>
    /// <param name="entity">The entity to map.</param>
    /// <returns>Result object containing the mapped dto.</returns>
    protected abstract Task<TDto> MapEntityToDto(TEntity entity, bool withValidation);

    /// <summary>
    /// Encodes an integer ID using Hashids.
    /// </summary>
    /// <param name="id">The integer ID to encode.</param>
    /// <returns>The encoded ID as a string.</returns>
    protected string EncodeId(int id)
    {
        return _hashids.Encode(id);
    }

    /// <summary>
    /// Decodes an encoded ID using Hashids.
    /// </summary>
    /// <param name="encodedId">The encoded ID to decode.</param>
    /// <returns>The decoded integer ID.</returns>
    protected int DecodeId(string encodedId)
    {
        var numbers = _hashids.Decode(encodedId);
        // TODO
        // if (numbers.Length < 1) throw new InvalidEncodedIdException(encodedId);
        if (numbers.Length < 1) throw new Exception($"Invalid encoding for id: {encodedId}");
        return numbers[0];
    }
}