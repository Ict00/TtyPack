using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class ByteUtilModule : LuaRuntimeModule
{
    private static readonly LuaFunction BytesToTextFunction = new ((context, _) =>
    {
        var a = context.GetArgument(0).Read<byte[]>();
        return new(context.Return(Encoding.UTF8.GetString(a)));
    });
    
    private static readonly LuaFunction BytesToTable = new ((context, _) =>
    {
        var a = context.GetArgument(0).Read<byte[]>();
        var table = new LuaTable();

        int i = 1;
        foreach (var b in a)
        {
            table[i++] = b;
        }
        
        return new(context.Return(table));
    });
    
    private static readonly LuaFunction TableToBytes = new ((context, _) =>
    {
        var a = context.GetArgument(0).Read<LuaTable>();
        List<byte> bytes = [];
        foreach (var i in a)
        {
            bytes.Add((byte)i.Value.Read<int>());
        }
        
        return new(context.Return(LuaValue.FromObject(bytes.ToArray())));
    });
    
    
    private static readonly LuaFunction TextToBytesFunction = new ((context, _) =>
    {
        var a = context.GetArgument(0).Read<string>();
        return new(context.Return(LuaValue.FromObject(Encoding.UTF8.GetBytes(a))));
    });
    
    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["bytes2str"] = BytesToTextFunction,
            ["str2bytes"] = TextToBytesFunction,
            ["bytes2table"] = BytesToTable,
            ["table2bytes"] = TableToBytes,
        };

        return table;
    }

    public override string GetModuleId() => "byteutil";
}