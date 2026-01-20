using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class EventModule : LuaRuntimeModule
{
    private static Dictionary<string, List<LuaFunction>> _events = new();
    
    private static readonly LuaFunction CreateEvent = new((context, _) =>
    {
        var name = context.GetArgument<string>(0);

        _events.TryAdd(name, []);
            
        return new(context.Return(LuaValue.Nil));
    });
    
    private static readonly LuaFunction SubscribeEvent = new((context, _) =>
    {
        var name = context.GetArgument<string>(0);
        var function = context.GetArgument<LuaFunction>(1);

        if (_events.ContainsKey(name))
        {
            _events[name].Add(function);
        }

        return new(context.Return(LuaValue.Nil));
    });
    
    private static readonly LuaFunction UnsubscribeEvent = new((context, _) =>
    {
        var name = context.GetArgument<string>(0);
        var function = context.GetArgument<LuaFunction>(1);

        if (_events.ContainsKey(name))
        {
            return new(context.Return(_events[name].Remove(function)));
        }

        return new(context.Return(false));
    });
    
    private static readonly LuaFunction CallEvent = new(async (context, _) =>
    {
        var name = context.GetArgument<string>(0);
            
            
        if (!_events.ContainsKey(name))
        {
            return context.Return(LuaValue.Nil);
        }
            
        List<LuaValue> args = [];
        for (int i = 1; i < context.ArgumentCount; i++)
        {
            var arg = context.GetArgument(i);
            args.Add(arg);
        }
            
        LuaValue[] arrayArgs = args.ToArray();
        args.Clear();
            
        foreach (var func in _events[name])
        {
            await context.State.CallAsync(func, arrayArgs);
        }

        return context.Return(LuaValue.Nil);
    });

    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["create"] = CreateEvent,
            ["subscribe"] = SubscribeEvent,
            ["unsubscribe"] = UnsubscribeEvent,
            ["call"] = CallEvent,
        };

        return table;
    }

    public override string GetModuleId() => "event";
}