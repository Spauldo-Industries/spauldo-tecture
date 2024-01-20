namespace spauldo_techture;
public interface IDto
{
    string Id { get; }
}

public abstract class Dto : IDto
{
    public virtual string Id { get; set; }
}