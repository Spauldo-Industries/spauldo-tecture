using HashidsNet;

namespace spauldo_techture;
public interface ILogicHashId
{
    string EncodeId(int id);
    int DecodeId(string encodedId);
}

public abstract class LogicHashId(IHashids hashids)
{
    private readonly IHashids _hashids = hashids;

    public virtual string EncodeId(int id)
    {
        return _hashids.Encode(id);
    }

    public virtual int DecodeId(string encodedId)
    {
        var numbers = _hashids.Decode(encodedId);
        // TODO
        // if (numbers.Length < 1) throw new InvalidEncodedIdException(encodedId);
        if (numbers.Length < 1) throw new Exception($"Invalid encoding for id: {encodedId}");
        return numbers[0];
    }
}