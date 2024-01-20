namespace spauldo_techture;
public interface IMapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
    where TTarget : class
{
    Task<MapperYonHash<TEntity, TModel, TDto, TAudit>> GetMapper();
    IMapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget> WithValidation();
    Task<TTarget> Map();
    Task<List<TTarget>> MapList();
}

public class MapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget>
: IMapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
    where TTarget : class
{
    private readonly MapperFactory _mapperFactory;
    private readonly MapperYonHash<TEntity, TModel, TDto, TAudit> _mapper;
    private readonly object _source;
    private readonly List<object> _sourceList;
    private bool _withValidation = false;

    public MapperYonHashBuilder(MapperFactory mapperFactory)
    {
        _mapperFactory = mapperFactory ?? throw new ArgumentNullException(nameof(mapperFactory));
    } 

    public MapperYonHashBuilder(MapperYonHash<TEntity, TModel, TDto, TAudit> mapper, object source)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public MapperYonHashBuilder(MapperYonHash<TEntity, TModel, TDto, TAudit> mapper, List<object> sourceList)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _sourceList = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
    }

    public async Task<MapperYonHash<TEntity, TModel, TDto, TAudit>> GetMapper()
    {
        return await _mapperFactory.GetMapper<TEntity, TModel, TDto, TAudit>();
    }

    public IMapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TTarget> WithValidation()
    {
        _withValidation = true;
        return this;
    }

    public async Task<TTarget> Map()
    {
        return await _mapper.Map<TTarget>(_source, _withValidation);
    }

    public async Task<List<TTarget>> MapList()
    {
        return await _mapper.MapList<TTarget>(_sourceList, _withValidation);
    }
}