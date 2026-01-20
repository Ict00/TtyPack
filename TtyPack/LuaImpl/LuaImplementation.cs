using System.Reflection;
using Lua;
using Lua.Standard;
using TtyPack.Modules;
using TtyPackLib;

namespace TtyPack;

public static class LuaImplementation
{
    private static List<LuaRuntimeModule> AllModules = [];
    
    public static void Init(LuaState state, App targetApp)
    {
        state.OpenTableLibrary();
        state.OpenStringLibrary();
        state.OpenBasicLibrary();
        state.OpenCoroutineLibrary();
        state.OpenMathLibrary();
        state.OpenBitwiseLibrary();
        
        state.Environment["print"] = LuaValue.Nil;
        state.Environment["dofile"] = LuaValue.Nil;
        state.Environment["loadfile"] = LuaValue.Nil;
        state.Environment["load"] = LuaValue.Nil;
        state.Environment["require"] = LuaValue.Nil;
        
        if (AllModules.Count == 0)
        {
            AllModules =
            [
                new AssetModule(),
                new ByteUtilModule(),
                new ConsoleModule(),
                new EventModule(),
                new PermissionModule(),
                new ScriptModule(),
                new StorageModule(),
                new SystemModule()
            ];
        }

        foreach (var i in AllModules)
        {
            state.Environment[i.GetModuleId()] = i.GetTable(targetApp);
        }
    }

    public static void CleanUp()
    {
        AllModules.Clear();
    }
}