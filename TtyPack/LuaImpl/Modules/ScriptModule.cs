using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class ScriptModule : LuaRuntimeModule
{
    private App _assets = null!;
    private readonly LuaFunction ScriptRun;
    private readonly LuaFunction ScriptCall;

    public ScriptModule()
    {
        ScriptRun = new(async (context, _) =>
        {
            var script = context.GetArgument<string>(0);
            var result = await context.State.DoStringAsync(script);
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

            var result = await context.State.DoStringAsync(script);
            return context.Return(result);
        });
    }

    public override LuaTable GetTable(App app)
    {
        _assets = app;
        
        var table = new LuaTable
        {
            ["run"] = ScriptRun,
            ["call"] = ScriptCall,
        };

        return table;
    }

    public override string GetModuleId() => "script";
}