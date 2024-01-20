using Microsoft.AspNetCore.Http;

namespace spauldo_techture;
public interface IModel
{
    int Id { get; }
    string CreateBy { get; }
    DateTime CreateDate { get; }
    string ModifyBy { get; }
    DateTime ModifyDate { get; }
}

public abstract class Model : IModel
{
    public abstract int Id { get; }
    public virtual string CreateBy { get; set; }
    public virtual DateTime CreateDate { get; set; }
    public virtual string ModifyBy { get; set; }
    public virtual DateTime ModifyDate { get; set; }

    public virtual void Initialize(IHttpContextAccessor context)
    {
        var name = context.HttpContext?.User?.Identity?.Name ?? throw new UnauthorizedAccessException($"Current user context cannot be null.");
        CreateDate = DateTime.Now;
        CreateBy =  name;
        ModifyDate = DateTime.Now;
        ModifyBy = name;
    }
}