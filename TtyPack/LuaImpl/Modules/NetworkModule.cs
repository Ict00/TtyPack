using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Lua;
using TtyPack.Bridges;
using TtyPackLib;

namespace TtyPack.Modules;

public class NetworkModule  : LuaRuntimeModule
{
    private HttpClient _httpClient = new();

    private readonly LuaFunction HttpGet;
    private readonly LuaFunction HttpPost;
    
    public NetworkModule()
    {
        HttpGet = new(async (context, _) =>
        {
            var uri = context.GetArgument<string>(0);
            try
            {
                var result = await _httpClient.GetAsync(uri);
                var str = await result.Content.ReadAsStringAsync();

                var returnTable = new LuaTable
                {
                    ["status"] = (int)result.StatusCode,
                    ["body"] = str
                };

                return context.Return(returnTable);
            }
            catch
            {
                return context.Return(LuaValue.Nil);
            }
        });

        HttpPost = new(async (context, _) =>
        {
            var uri = context.GetArgument<string>(0);
            var content = context.GetArgument(1);
            var contentType = context.ArgumentCount == 3 ? context.GetArgument<string>(2) : null;
            HttpContent httpContent;
            
            if (content.TryRead<LuaTable>(out var table) && contentType == null)
            {
                httpContent = new StringContent(JsonModule.JsonSerialize(table),  Encoding.UTF8, "application/json");
                
            }
            else
            {
                httpContent = new StringContent(content.Read<string>(), Encoding.UTF8, contentType);
            }

            try
            {
                var result = await _httpClient.PostAsync(uri, httpContent);
                var str = await result.Content.ReadAsStringAsync();

                var returnTable = new LuaTable
                {
                    ["status"] = (int)result.StatusCode,
                    ["body"] = str
                };

                return context.Return(returnTable);
            }
            catch
            {
                return  context.Return(LuaValue.Nil);
            }
        });
    }

    public override LuaTable GetTable(App app)
    {
        var tcpTable = new LuaTable()
        {
            
        };

        var httpTable = new LuaTable()
        {
            ["get"] = HttpGet,
            ["post"] = HttpPost,
        };

        var table = new LuaTable
        {
            ["http"] = httpTable,
            ["tcp"] = tcpTable
        };
        
        return table;
    }

    public override string GetModuleId() => "network";
}