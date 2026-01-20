using System.Diagnostics;
using Lua;
using Lua.Standard;
using TtyPackLib;

namespace TtyPack.Modules;

public class SystemModule : LuaRuntimeModule
{
    public static string[] Args = null!;
    
    private static readonly LuaFunction DirContents = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.read")) return new (context.Return(LuaValue.Nil));
        string dir = context.GetArgument<string>(0);
        string filter = context.ArgumentCount > 1 ? context.GetArgument<string>(1) : "*";
        SearchOption option = context.ArgumentCount == 3 ? (context.GetArgument<bool>(2) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : SearchOption.TopDirectoryOnly;
        
        var table = new LuaTable();
        int i = 1;
        foreach (var entry in Directory.EnumerateFileSystemEntries(dir, filter, option))
        {
            table[i++] = entry;
        }
        
        return new(context.Return(table));
    });
    
    private static readonly LuaFunction FileExists = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.read")) return new (context.Return(LuaValue.Nil));
        
        var filename = context.GetArgument<string>(0);
        
        return new(context.Return(File.Exists(filename)));
    });
    
    private static readonly LuaFunction DirExists = new( (context, _) =>
    {
        if (!Permissions.CheckPermission("fs.read")) return new (context.Return(LuaValue.Nil));
        
        var dirname = context.GetArgument<string>(0);
        
        return new(context.Return(Directory.Exists(dirname)));
    });
    
    private static readonly LuaFunction ReadTextFile = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.read")) return new (context.Return(LuaValue.Nil));
        
        var filename = context.GetArgument<string>(0);
        var text = File.ReadAllText(filename);
        
        return new(context.Return(text));
    });
    
    private static readonly LuaFunction ReadBytesTextFile = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.read")) return new (context.Return(LuaValue.Nil));
        
        var filename = context.GetArgument<string>(0);
        var bytes = File.ReadAllBytes(filename);
        
        return new(context.Return(LuaValue.FromObject(bytes)));
    });
    
    private static readonly LuaFunction WriteTextFile = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.write")) return new (context.Return(LuaValue.Nil));
        
        var filename = context.GetArgument<string>(0);
        var text = context.GetArgument<string>(1);
        File.WriteAllText(filename, text);
        
        return new(context.Return(LuaValue.Nil));
    });
    
    private static readonly LuaFunction WriteBytesTextFile = new((context, _) =>
    {
        if (!Permissions.CheckPermission("fs.write")) return new (context.Return(LuaValue.Nil));
        
        var filename = context.GetArgument<string>(0);
        var bytes = context.GetArgument<byte[]>(1);
        File.WriteAllBytes(filename, bytes);
        
        return new(context.Return(LuaValue.Nil));
    });
    
    private static readonly LuaFunction GetArgv = new((context, _) =>
    {
        if (!Permissions.CheckPermission("os.argv")) return new (context.Return(LuaValue.Nil));
        
        var table = new LuaTable();
        int i = 1;
        foreach (var c in Args)
        {
            table[i++] = c;
        }
        
        return new(context.Return(table));
    });
    
    private static readonly LuaFunction OsRun = new(async (context, _) =>
    {
        if (!Permissions.CheckPermission("os.run")) return context.Return(LuaValue.Nil);
        
        var cmd = context.GetArgument<string>(0);
        await Process.Start(cmd).WaitForExitAsync();
        
        return context.Return(LuaValue.Nil);
    });
    
    private static readonly LuaFunction UserName = new((context, _) =>
    {
        if (!Permissions.CheckPermission("os.identity")) return new(context.Return(LuaValue.Nil));
        
        return new(context.Return(Environment.UserName));
    });
    
    private static readonly LuaFunction GetCurrentDirectory = new((context, _) =>
    {
        if (!Permissions.CheckPermission("os.identity")) return new(context.Return(LuaValue.Nil));
        
        return new(context.Return(Environment.CurrentDirectory));
    });
    
    private static readonly LuaFunction CombinePaths = new((context, _) =>
    {
        List<string> paths = [];

        foreach (var i in context.Arguments)
        {
            paths.Add(i.Read<string>());
        }
        
        return new(context.Return(Path.Combine([..paths])));
    });
    
    private static readonly LuaFunction Time = new((context, c) => OperatingSystemLibrary.Instance.Time(new LuaFunctionExecutionContext()
    {
        ArgumentCount = context.ArgumentCount,
        ReturnFrameBase = context.ReturnFrameBase,
        State = context.State,
    }, c));
    
    private static readonly LuaFunction Date = new((context, c) => OperatingSystemLibrary.Instance.Date(new LuaFunctionExecutionContext()
    {
        ArgumentCount = context.ArgumentCount,
        ReturnFrameBase = context.ReturnFrameBase,
        State = context.State,

    }, c));

    public SystemModule()
    {
        Permissions.AddPermission("fs.read");
        Permissions.AddPermission("fs.write");
        Permissions.AddPermission("os.identity");
        Permissions.AddPermission("os.run");
        Permissions.AddPermission("os.argv");
    }

    public override LuaTable GetTable(App app)
    {
        var table = new LuaTable
        {
            ["read_dir"] = DirContents,
            ["file_exists"] = FileExists,
            ["dir_exists"] = DirExists,
            ["read_file_str"] = ReadTextFile,
            ["read_file_text"] = ReadTextFile,
            ["read_file_bytes"] = ReadBytesTextFile,
            ["write_file_str"] = WriteTextFile,
            ["write_file_text"] = WriteTextFile,
            ["write_file_bytes"] = WriteBytesTextFile,
            ["get_argv"] = GetArgv,
            ["os_run"] = OsRun,
            ["get_username"] = UserName,
            ["get_current_dir"] = GetCurrentDirectory,
            ["combine_paths"] = CombinePaths,
            ["time"] = Time,
            ["date"] = Date,
            
        };
        return table;
    }

    public override string GetModuleId() => "system";
}