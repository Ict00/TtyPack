using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class StorageModule : LuaRuntimeModule
{
    private static readonly LuaFunction Commit = new((context, _) => new(context.Return(Storage.Commit())));
    
    private static readonly LuaFunction HasKey = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.LocalHas(id)));
    });
    
    private static readonly LuaFunction ClearAll = new((context, _) =>
    {
        Storage.ClearAll();
        return new(context.Return(LuaValue.Nil));
    });
    
    private static readonly LuaFunction RemoveKey = new((context, _) =>
    { 
        string id = context.GetArgument<string>(0);
        
        return new(context.Return(Storage.Remove(id)));
    });

    private static readonly LuaFunction GetBytes = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(LuaValue.FromObject(Storage.GetByteArray(id))));
    });
    
    private static readonly LuaFunction GetTableFunction = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.GetTable(id)));
    });
    
    private static readonly LuaFunction GetString = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.GetString(id)));
    });
    
    private static readonly LuaFunction GetInt = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.GetInt(id)));
    });
    
    private static readonly LuaFunction GetFloat = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.GetFloat(id)));
    });
    
    private static readonly LuaFunction GetBool = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        return new(context.Return(Storage.GetBool(id)));
    });

    private static readonly LuaFunction Put = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        var value = context.GetArgument(1);

        if (value.TryRead(out byte[] bytes))
        {
            Storage.PutByteArray(id, bytes);
        }

        if (value.TryRead(out string text))
        {
            Storage.PutString(id, text);
        }

        if (value.TryRead(out int intValue))
        {
            Storage.PutInt(id, intValue);
        }

        if (value.TryRead(out float floatValue))
        {
            Storage.PutFloat(id, floatValue);
        }

        if (value.TryRead(out bool boolValue))
        {
            Storage.PutBool(id, boolValue);
        }

        if (value.TryRead(out LuaTable table))
        {
            Storage.PutTable(id, table);
        }
        
        return new(context.Return(LuaValue.Nil));
    });

    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["commit"] = Commit,
            ["clear_all"] = ClearAll,
            ["put"] = Put,
            ["has"] = HasKey,
            ["remove"] = RemoveKey,
            ["get_int"] = GetInt,
            ["get_float"] = GetFloat,
            ["get_bool"] = GetBool,
            ["get_bytes"] = GetBytes,
            ["get_string"] = GetString,
            ["get_table"] = GetTableFunction
        };

        return table;
    }

    public override string GetModuleId() => "storage";
}