using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class PermissionModule : LuaRuntimeModule
{
    private readonly LuaFunction CheckPermission = new((context, _) =>
    {
        var has = Permissions.CheckPermission(context.GetArgument<string>(0));
        return new (context.Return(has));
    });
    
    private readonly LuaFunction RequestPermission = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        string? reason = null;

        if (context.ArgumentCount == 2)
        {
            reason = context.GetArgument<string>(1);
        }
        Permissions.AskPermission(id, reason);
        
        return new (context.Return(LuaValue.Nil));
    });
    
    private readonly LuaFunction AskPermission = new((context, _) =>
    {
        string id = context.GetArgument<string>(0);
        string? reason = null;

        if (context.ArgumentCount == 2)
        {
            reason = context.GetArgument<string>(1);
        }
        
        return new (context.Return(Permissions.AskPermissionRightaway(id, reason)));
    });

    private readonly LuaFunction AddPermission = new((context, _) =>
    {
        var id =  context.GetArgument<string>(0);
        Permissions.AddPermission(id);
        return new (context.Return(LuaValue.Nil));
    });

    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["has"] = CheckPermission,
            ["request"] = RequestPermission,
            ["ask"] = AskPermission,
            ["add"] = AddPermission,
        };

        return table;
    }

    public override string GetModuleId() => "permission";
}