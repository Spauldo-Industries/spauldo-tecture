using System.Reflection;

namespace spauldo_techture;
public static class ModelHelper
{
    public static bool TryGetIntPropertyValue<TEntity>(TEntity entity, string propertyName, out int idValue) where TEntity : class
    {
        var propertyInfo = GetPropertyInfo<TEntity>(propertyName);

        if (propertyInfo != null)
        {
            var idObject = propertyInfo.GetValue(entity);

            if (idObject != null && int.TryParse(idObject.ToString(), out idValue))
            {
                return true;
            }
        }

        idValue = 0;
        return false;
    }

    public static object GetPropertyValue<TDto>(TDto dto, string propertyName) where TDto : class
    {
        var propertyInfo = GetPropertyInfo<TDto>(propertyName);
        return propertyInfo?.GetValue(dto);
    }

    private static PropertyInfo GetPropertyInfo<TType>(string propertyName) where TType : class
    {
        return typeof(TType).GetProperty(propertyName);
    }
}