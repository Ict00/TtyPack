using System.Text;
using Lua;
using TtyPackLib;

namespace TtyPack.Modules;

public class AssetModule : LuaRuntimeModule
{
    private App _assets = null!;
    private readonly LuaFunction GetAsset;
    private readonly LuaFunction HasAsset;
    private readonly LuaFunction PushAsset;

    public AssetModule()
    {
        GetAsset = new LuaFunction((context, _) =>
        {
            var assetName = context.Arguments[0].Read<string>();

            if (_assets.HasAsset(assetName))
            {
                Asset asset = _assets.GetAsset(assetName)!;
                var table = new LuaTable();
                table["id"] = asset.Id;
                table["type"] = asset.AssetType.AsString();
                
                bool asText = false;
            
                if (context.ArgumentCount == 2)
                {
                    asText = context.Arguments[1].Read<bool>();
                }

                if (!asText)
                {
                    switch (asset.AssetType)
                    {
                        case AssetType.Blob:
                            table["data"] = LuaValue.FromObject(asset.Data);
                            break;
                        case AssetType.Script:
                            table["data"] = Encoding.UTF8.GetString(asset.Data);
                            break;
                        case AssetType.TextBlob: goto case AssetType.Script;
                    }
                }
                else
                {
                    table["data"] = Encoding.UTF8.GetString(asset.Data);
                }

                return new(context.Return(table));
            }

            return new(context.Return(LuaValue.Nil));

        });

        HasAsset = new LuaFunction((context, _) =>
        {
            var assetName = context.Arguments[0].Read<string>();
            return new(context.Return(_assets.HasAsset(assetName)));
        });

        PushAsset = new LuaFunction((context, _) =>
        {
            var assetName = context.Arguments[0].Read<string>();
            var assetValue = context.Arguments[1];
            byte[] data = [];
            AssetType assetType = AssetType.Blob;

            if (assetValue.Type == LuaValueType.String)
            {
                data = Encoding.UTF8.GetBytes(assetValue.Read<string>());
                assetType = AssetType.TextBlob;
            }

            if (assetValue.Type == LuaValueType.LightUserData)
            {
                data = assetValue.Read<byte[]>();
            }
            
            _assets.Assets.Add(new(assetName, assetType, data));
            
            return new(context.Return(LuaValue.Nil));
        });
    }

    public override LuaTable GetTable(App app)
    {
        _assets = app;
        
        var table = new LuaTable
        {
            ["get"] = GetAsset,
            ["has"] = HasAsset,
            ["push"] = PushAsset,
        };

        return table;
    }

    public override string GetModuleId() => "assets";
}