using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class ConsoleModule : LuaRuntimeModule
{
    private static readonly LuaFunction PrintFunction = new ((context, _) => {
        for (int i = 0; i < context.ArgumentCount; i++)
        {
            try
            {
                Console.Write(context.GetArgument<object>(i));
            }
            catch
            {
                Console.Write("nil");
            }
        }

        return new(0);
    });
    
    private static readonly LuaFunction PrintlnFunction = new ((context, _) => {
        for (int i = 0; i < context.ArgumentCount; i++)
        {
            try
            {
                Console.Write(context.GetArgument<object>(i));
            }
            catch
            {
                Console.Write("nil");
            }
        }
        Console.Write('\n');
        
        return new(0);
    });

    private static readonly LuaFunction Readline = new((context, _) => new(context.Return(Console.ReadLine() ?? "")));

    private static readonly LuaFunction Readkey = new((context, _) =>
    {
        bool intercept = false;
        if (context.ArgumentCount > 0)
        {
            intercept = context.GetArgument<bool>(0);
        }

        var key = Console.ReadKey(intercept);

        LuaTable result = new();

        result["char"] = $"{key.KeyChar}";
        result["control"] = key.Modifiers.HasFlag(ConsoleModifiers.Control);
        result["shift"] = key.Modifiers.HasFlag(ConsoleModifiers.Shift);
        result["alt"] = key.Modifiers.HasFlag(ConsoleModifiers.Alt);
        result["key"] = (int)key.Key;

        
        return new(context.Return(result));
    });

    private static readonly LuaFunction Height = new((context, _) => new(context.Return(Console.BufferHeight)));
    private static readonly LuaFunction Width = new((context, _) => new(context.Return(Console.BufferWidth)));

    private static readonly LuaFunction GetCursor = new((context, _) =>
    {
        var pos = Console.GetCursorPosition();
        var table = new LuaTable()
        {
            ["x"] = pos.Left,
            ["y"] = pos.Top,
        };
        
        return new(context.Return(table));
    });

    private static readonly LuaFunction ClearFunction = new((context, _) =>
    {
        Console.Clear();
        return new(context.Return(LuaValue.Nil));
    });
    
    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["print"] = PrintFunction,
            ["println"] = PrintlnFunction,
            ["readln"] = Readline,
            ["readkey"] = Readkey,
            ["get_height"] = Height,
            ["get_width"] = Width,
            ["get_cursor"] = GetCursor,
            ["clear"] = ClearFunction,
        };

        return table;
    }

    public override string GetModuleId() => "console";
}