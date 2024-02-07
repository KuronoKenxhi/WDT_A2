using Newtonsoft.Json;
using s3901335_a2.Models;

public class AccountTypeStringToAccountTypeEnumConverter : JsonConverter<AccountType>
{
    public override void WriteJson(JsonWriter writer, AccountType value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override AccountType ReadJson(JsonReader reader, Type objectType, AccountType existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        // The type is a string in the JSON.
        var type = (string)reader.Value;

        // Convert the string to an enum.
        return type switch
        {
            "S" => AccountType.Savings,
            "C" => AccountType.Checkings,
            _ => throw new InvalidOperationException($"Unknown AccountType: {type}")
        };
    }
}