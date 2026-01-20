using MessagePack;
using Newtonsoft.Json;

namespace TtyPackLib;

[MessagePackObject]
public class App(string name, string author, string desc, List<Asset> assets)
{
    [Key(0)]
    public string Name = name;
    [Key(1)]
    public string Author = author;
    [Key(2)]
    public string Description = desc;
    [Key(3)]
    public List<Asset> Assets { get; set; } = assets;

    public bool HasAsset(string id)
    {
        return Assets.Any(x => x.Id == id);
    }

    public bool HasAsset(AssetType assetType, string id)
    {
        return Assets.Any(x => x.AssetType == assetType && x.Id == id);
    }

    public Asset? GetAsset(string id)
    {
        return Assets.FirstOrDefault(x => x.Id == id);
    }

    public Asset? GetAsset(AssetType assetType, string id)
    {
        return Assets.FirstOrDefault(x => x.AssetType == assetType && x.Id == id);
    }
    
    private static Dictionary<string, AssetType> _typesBasedOnExtensions = new()
    {
        [".lua"] = AssetType.Script,
        [".xml"] = AssetType.TextBlob,
        [".json"] = AssetType.TextBlob,
        [".txt"] = AssetType.TextBlob
    };

    public static App? FromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File doesn't exist: {filePath}");
            return null;
        }

        try
        {
            var bytes = File.ReadAllBytes(filePath);
            var a = MessagePackSerializer.Deserialize<App>(bytes);
            return a;
        }
        catch(Exception e)
        {
            Console.WriteLine($"Exception parsing file: {e.Message}");
            return null;
        }
    }

    public static App? FromDirectory(string appPath)
    {
        if (!Directory.Exists(appPath))
        {
            Console.WriteLine($"app path doesn't exist: {appPath}"); 
            return null;
        }

        if (!File.Exists($"{appPath}/idmap.json"))
        {
            Console.WriteLine($"app path is not valid: no idmap.json");
            return null;
        }
        
        if (!File.Exists($"{appPath}/manifest.json"))
        {
            Console.WriteLine($"app path is not valid: no manifest.json");
            return null;
        }
        
        Manifest? manifest = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText($"{appPath}/manifest.json"));
        var config = File.ReadAllText($"{appPath}/idmap.json").GetIdMap();
        
        manifest ??= new Manifest("nil", "nil", "nil");
        
        List<Asset> assets = [];
        
        foreach (var v in config)
        {
            string filePath = $"{appPath}/{v.Value}";
            var ext = Path.GetExtension(filePath);
            
            if (string.IsNullOrEmpty(ext)) continue;
            
            AssetType assetType = AssetType.Blob;
            
            if (_typesBasedOnExtensions.TryGetValue(ext, out var type))
            {
                assetType = type;
            }

            try
            {
                assets.Add(new Asset(v.Key, assetType, File.ReadAllBytes(filePath)));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception reading '{filePath}' [{v.Key}]: {e.Message}");
            }
        }
        
        return new App(manifest.Name, manifest.Author, manifest.Description,assets);
    }
}

[MessagePackObject]
public class Asset(string id, AssetType assetType, byte[] data)
{
    [Key(0)] public string Id = id;
    [Key(1)] public AssetType AssetType = assetType;
    [Key(2)] public byte[] Data = data;
}

public enum AssetType
{
    Script,
    Blob,
    TextBlob
}

public static class AssetTypeExtensions
{
    public static string AsString(this AssetType assetType)
    {
        switch (assetType)
        {
            case AssetType.Script:
                return "script";
            case AssetType.Blob:
                return "blob";
            case AssetType.TextBlob:
                return "textblob";
        }

        return "nil";
    }

}