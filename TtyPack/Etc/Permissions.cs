using Lua;
using MessagePack;
using TtyPackLib;

namespace TtyPack;

public static class Permissions
{
    private static App _app = null!;
    private static List<Permission> _permissions = [];
    
    public static void Init(App app)
    {
        _app = app;
        try
        {
            var data = File.ReadAllBytes(Path.Combine(Storage.AppStoragePath, "permissions"));
            _permissions = MessagePackSerializer.Deserialize<List<Permission>>(data);
        }
        catch
        {
            _permissions = [];
        }
    }

    private static Permission? Get(string id)
    {
        return _permissions.FirstOrDefault(p => p.Id == id);
    }

    public static bool CheckPermission(string id)
    {
        var p = Get(id);
        if (p == null) return false;

        if (p.Level == PermissionLevel.Deny) return false;
        if (p.Level == PermissionLevel.AllowOnce)
        {
            p.Level = PermissionLevel.Deny;
            Commit();
        }

        return true;
    }

    public static bool AskPermissionRightaway(string id, string? reason = null)
    {
        var p = Get(id);
        if (p == null) return false;
        
        AskPermission(id, reason);
        return CheckPermission(id);
    }
    
    public static void AskPermission(string id, string? reason = null)
    {
        var p = Get(id);
        if (p == null) return;
        if (p.Level == PermissionLevel.Allow || p.Level == PermissionLevel.AllowOnce) return;
        
        Console.WriteLine($"{_app.Name} asks for {id} permission{(reason == null ? "" : $" for: {reason}")}\n[A] - Allow Always [O] - Allow Once [D] - Deny");
        PermissionLevel level = PermissionLevel.Deny;

        while (true)
        {
            var k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.A)
            {
                level = PermissionLevel.Allow;
                break;
            }
            if (k.Key == ConsoleKey.O)
            {
                level = PermissionLevel.AllowOnce;
                break;
            }
            if (k.Key == ConsoleKey.D)
            {
                level = PermissionLevel.Deny;
                break;
            }
        }
        
        p.Level = level;
        Commit();
    }

    public static void AddPermission(string id)
    {
        var p = Get(id);
        if (p != null) return;
        
        _permissions.Add(new(id, PermissionLevel.Deny));
        Commit();
    }

    private static void Commit()
    {
        var data = MessagePackSerializer.Serialize(_permissions);
        File.WriteAllBytes(Path.Combine(Storage.AppStoragePath, "permissions"), data);
    }
}