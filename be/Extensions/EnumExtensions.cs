namespace TaskSystem.Extensions;

public static class EnumExtensions
{
    private static readonly Dictionary<Type, Dictionary<string, string>> EnumValues = new();

    public static void SetEnumValues<T>(Dictionary<string, string> values)
    {
        var type = typeof(T);
        if (!EnumValues.ContainsKey(type))
        {
            EnumValues[type] = new Dictionary<string, string>();
        }

        values.ToList().ForEach(pair =>
        {
            if (Enum.IsDefined(type, pair.Key))
            {
                EnumValues[type][pair.Key] = pair.Value;
            }
        });
    }

    public static T? GetEnumValue<T>(string value) where T : Enum
    {
        var type = typeof(T);
        if (!EnumValues.ContainsKey(type))
        {
            return default;
        }

        return EnumValues[type].Where(e => e.Value == value)
            .Select(e => (T)Enum.Parse(type, e.Key))
            .FirstOrDefault();
    }

    public static string GetStringValue<T>(this T? value) where T : struct, Enum
    {
        if (!value.HasValue)
        {
            return "";
        }

        var type = typeof(T);
        var name = Enum.GetName(type, value.Value);
        if (EnumValues.ContainsKey(type) && name is not null && EnumValues[type].ContainsKey(name))
        {
            return EnumValues[type][name];
        }
        return name ?? "";
    }
}