using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tools.Serialization;

public class SensitiveDataResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(
        MemberInfo member,
        MemberSerialization memberSerialization
    )
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (member is PropertyInfo propertyInfo)
        {
            var isSensitive = Attribute.IsDefined(propertyInfo, typeof(SensitiveDataAttribute));

            if (isSensitive)
            {
                property.ValueProvider = new StringValueProvider("SensitiveData");
            }
        }

        return property;
    }
}
