using Newtonsoft.Json;

namespace TtyPack.Bridges;

public class DataUnitConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(DataUnit).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.Null:
                return new DataNil();
            case JsonToken.String:
                var stringValue = (string)reader.Value;
                if (stringValue == "nil" || stringValue == "null")
                    return new DataNil();
                return new DataString(stringValue);
                
            case JsonToken.Boolean:
                return new DataBool((bool)reader.Value);
                
            case JsonToken.Integer:
            case JsonToken.Float:
                return new DataNumber(Convert.ToDouble(reader.Value));
                
            case JsonToken.StartObject:
                var dict = serializer.Deserialize<Dictionary<string, DataUnit>>(reader);
                return new DataTable(dict);
                
            case JsonToken.StartArray:
                var list = serializer.Deserialize<List<DataUnit>>(reader);
                return new DataList(list);
            default:
                return new DataNil();
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is DataNil)
        {
            writer.WriteValue("null");
        }
        else if (value is DataString str)
        {
            writer.WriteValue(str.Value);
        }
        else if (value is DataBool b)
        {
            writer.WriteValue(b.Value);
        }
        else if (value is DataNumber num)
        {
            writer.WriteValue(num.Value);
        }
        else if (value is DataBytes bytes)
        {
            writer.WriteValue(Convert.ToBase64String(bytes.Value));
        }
        else if (value is DataTable table)
        {
            serializer.Serialize(writer, table.DataUnits);
        }
        else if (value is DataList list)
        {
            serializer.Serialize(writer, list.DataUnits);
        }
    }
}