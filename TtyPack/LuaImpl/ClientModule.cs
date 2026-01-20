using Lua;
using TtyPackLib;

namespace TtyPack;

public abstract class LuaRuntimeModule
{
    public abstract LuaTable GetTable(App app);
    public abstract string GetModuleId();
}