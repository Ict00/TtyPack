using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class ScriptModule : LuaRuntimeModule
{
    private App _assets = null!;
    private readonly LuaFunction ScriptRun;
    private readonly LuaFunction ScriptCall;
    private readonly LuaFunction BorrowFunction;

    private Dictionary<string, LuaTable> _borrowTable = [];

    public ScriptModule()
    {
        ScriptRun = new(async (context, _) =>
        {
            var script = context.GetArgument<string>(0);
            
            var anotherState = LuaState.Create();
            LuaImplementation.Init(anotherState, _assets);
            
            var result = await anotherState.DoStringAsync(script);
            return context.Return(result);
        });

        ScriptCall = new(async (context, _) =>
        {
            var scriptName = context.GetArgument<string>(0);
            var asset = _assets.GetAsset(scriptName);

            if (asset == null)
            {
                context.Return(LuaValue.Nil);
                return 1;
            }

            var script = Encoding.UTF8.GetString(asset.Data);
            var anotherState = LuaState.Create();
            LuaImplementation.Init(anotherState, _assets);

            var result = await anotherState.DoStringAsync(script);
            
            if (!_borrowTable.ContainsKey(scriptName))
                _borrowTable[scriptName] = anotherState.Environment;
            
            return context.Return(result);
        });
        
        BorrowFunction = new(async (context, _) =>
        {
            var scriptName = context.GetArgument<string>(0);
            var functionName = context.GetArgument<string>(1);
            
            var asset = _assets.GetAsset(scriptName);

            if (_borrowTable.TryGetValue(scriptName, out var table))
            {
                return context.Return(table[functionName]);
            }

            if (asset == null)
            {
                context.Return(LuaValue.Nil);
                return 1;
            }

            var script = Encoding.UTF8.GetString(asset.Data);
            
            var anotherState = LuaState.Create();
            LuaImplementation.Init(anotherState, _assets);

            await anotherState.DoStringAsync(script);
            
            _borrowTable[scriptName] = anotherState.Environment;
            
            return context.Return(anotherState.Environment[functionName]);
        });
    }

    public override LuaTable GetTable(App app)
    {
        _assets = app;
        
        var table = new LuaTable
        {
            ["run"] = ScriptRun,
            ["call"] = ScriptCall,
            ["borrow"] = BorrowFunction,
        };

        return table;
    }

    public override string GetModuleId() => "script";
}