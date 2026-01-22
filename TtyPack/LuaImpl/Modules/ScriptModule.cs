using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class ScriptModule : LuaRuntimeModule
{
    private App _assets = null!;
    private readonly LuaFunction ScriptRun;
    private readonly LuaFunction ScriptCall;
    private readonly LuaFunction ScriptSafe;
    private readonly LuaFunction ScriptImport;
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

        ScriptSafe = new(async (context, _) =>
        {
            var function = context.GetArgument<LuaFunction>(0);
            LuaFunction? catchFunction = context.ArgumentCount == 2 ? context.GetArgument<LuaFunction>(1) : null;

            try
            {
                var result = await context.State.CallAsync(function, []);
                return context.Return(result);
            }
            catch (Exception e)
            {
                if (catchFunction != null)
                {
                    var exceptionTable = new LuaTable();
                    exceptionTable["message"] = e.Message;
                    if (e is LuaRuntimeException runtimeException)
                    {
                        exceptionTable["message"] = runtimeException.Message;
                        exceptionTable["traceback"] = runtimeException.LuaTraceback?.ToString() ?? LuaValue.Nil;
                        exceptionTable["source"] = runtimeException.ErrorObject;
                    }
                    
                    await context.State.CallAsync(catchFunction, [exceptionTable]);
                    return context.Return(LuaValue.Nil);
                }
                else
                {
                    return context.Return(LuaValue.Nil);
                }
            }
        });
        
        ScriptImport = new(async (context, _) =>
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

            await anotherState.DoStringAsync(script);

            foreach (var i in anotherState.Environment)
            {
                try
                {
                    if (!context.State.Environment.ContainsKey(i.Key))
                    {
                        context.State.Environment[i.Key] = i.Value;
                    }
                }
                catch { /* Ignore */}
            }
            
            return context.Return(LuaValue.Nil);
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
            ["import"] = ScriptImport,
            ["try_catch"] = ScriptSafe,
        };

        return table;
    }

    public override string GetModuleId() => "script";
}