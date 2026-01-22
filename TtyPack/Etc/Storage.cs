using System.Text;
using Lua;
using MessagePack;
using TtyPack.Bridges;
using TtyPackLib;

namespace TtyPack;

public static class Storage
{
    private static App _app = null!;

    private static List<string> _entries =
    [
        "permissions",
        "localstorage"
    ];

    private static Dictionary<string, byte[]> _localStorage = [];
    private static string _appStoragePath = string.Empty;
    public static string AppStoragePath => _appStoragePath;
    private static void InitStorageDirectory()
    {
        var baseDir = Environment.ProcessPath;
        if (baseDir == null)
        {
            Environment.Exit(-1);
        }

        baseDir = new FileInfo(baseDir).Directory!.FullName;
        
        var ttyStorage =  Path.Combine(baseDir, ".ttystorage");
        
        if (!Directory.Exists(ttyStorage))
        {
            Directory.CreateDirectory(ttyStorage);
        }
        
        var appStorage =  Path.Combine(ttyStorage, _app.Name);
        if (!Directory.Exists(appStorage))
        {
            Directory.CreateDirectory(appStorage);
        }
        
        _appStoragePath = appStorage;
        
        foreach (var i in _entries)
        {
            var file = Path.Combine(_appStoragePath, i);
            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }
        }
    }
    
    public static void Init(App app)
    {
        _app = app;
        
        InitStorageDirectory();
        InitLocalStorage();
    }
    
    private static void InitLocalStorage()
    {
        var localstorage = Path.Combine(_appStoragePath, "localstorage");
        try
        {
            var data = File.ReadAllBytes(localstorage);
            _localStorage = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(data);
        }
        catch
        {
            _localStorage = new Dictionary<string, byte[]>();
            try
            {
                File.WriteAllBytes(localstorage, MessagePackSerializer.Serialize(_localStorage));
            }
            catch { /* Ignore */ }
        }
    }
    
    public static bool LocalHas(string key) =>  _localStorage.ContainsKey(key);

    public static bool Remove(string key)
    {
        return _localStorage.Remove(key);
    }
    
    public static bool Commit()
    {
        var localstorage = Path.Combine(_appStoragePath, "localstorage");
        
        try
        {
            File.WriteAllBytes(localstorage, MessagePackSerializer.Serialize(_localStorage));
            return true;
        }
        catch { /* Ignore */ }
        return false;
    }
    public static void ClearAll() => _localStorage.Clear();
    public static void PutByteArray(string key, byte[] value) =>  _localStorage[key] = value;
    public static void PutString(string key, string value) => PutByteArray(key, Encoding.UTF8.GetBytes(value));
    public static void PutBool(string key, bool value) => PutByteArray(key, MessagePackSerializer.Serialize(value));
    public static void PutNumber(string key, double value) => PutByteArray(key, MessagePackSerializer.Serialize(value));
    public static void PutTable(string key, LuaTable value) => PutByteArray(key, MessagePackSerializer.Serialize(value.FromLua()));

    public static byte[] GetByteArray(string key)
    {
        if (_localStorage.TryGetValue(key, out var data))
        {
            return data;
        }

        return [];
    }

    public static string GetString(string key)
    {
        var  data = GetByteArray(key);
        try
        {
            return Encoding.UTF8.GetString(data);
        }
        catch
        {
            return "";
        }
    }
    
    public static LuaTable GetTable(string key)
    {
        var  data = GetByteArray(key);
        try
        {
            var table = MessagePackSerializer.Deserialize<DataUnit>(data);
            if (table is DataTable realTable) return realTable.FromData();
            if (table is DataList dataList) return dataList.FromDataList();
            return new LuaTable();
        }
        catch
        {
            return new();
        }
    }
    
    public static bool GetBool(string key)
    {
        var  data = GetByteArray(key);
        try
        {
            return MessagePackSerializer.Deserialize<bool>(data);
        }
        catch
        {
            return false;
        }
    }
    
    public static double GetNumber(string key)
    {
        var  data = GetByteArray(key);
        try
        {
            return MessagePackSerializer.Deserialize<double>(data);
        }
        catch
        {
            return 0;
        }
    }
}