using System.Text;
using Lua;
using Newtonsoft.Json;
using TtyPack.Bridges;
using TtyPackLib;

namespace TtyPack.Modules;

public class JsonModule  : LuaRuntimeModule
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Converters = new List<JsonConverter> { new DataUnitConverter() }
    };
    
    private static readonly LuaFunction SerializeTable = new((context, _) =>
    {
        var table = context.GetArgument<LuaTable>(0);

        return new(context.Return(JsonSerialize(table)));
    });
    
    private static readonly LuaFunction Deserialize = new((context, _) =>
    {
        var json = context.GetArgument<string>(0);
        
        return new(context.Return(JsonDeserialize(json)));
    });
    
    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["serialize"] = SerializeTable,
            ["deserialize"] = Deserialize,
        };
        
        return table;
    }

    public override string GetModuleId() => "json";

    public static string JsonSerialize(LuaTable table)
    {
        var json = JsonConvert.SerializeObject(table.FromLua(), Formatting.None, settings);
        return json;
    }

    public static LuaValue JsonDeserialize(string json)
    {
        var result = JsonConvert.DeserializeObject<DataUnit>(json,  settings)??new DataNil();
        return result.TryRead();
    }
}