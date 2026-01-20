using System.Net;
using System.Text;
using Lua;
using MessagePack;
using TtyPackLib;

namespace TtyPack;

public class Client
{
    private HttpClient _httpClient = new();
    
    public async Task<App?> GetAppFromUriAsync(string uri)
    {
        try
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
            if (response.StatusCode != HttpStatusCode.OK) return await Task.FromResult<App?>(null);

            byte[] buffer = await response.Content.ReadAsByteArrayAsync();

            try
            {
                App app = MessagePackSerializer.Deserialize<App>(buffer);
                return await Task.FromResult<App?>(app);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to get app from {uri}: {e.Message}");
            }

            return await Task.FromResult<App?>(null);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get app from {uri}: {e.Message}");
            return await Task.FromResult<App?>(null);
        }
    }

    public Task<App?> GetAppFromFileAsync(string path)
    {
        return Task.FromResult(App.FromFile(path));
    } 
    
    public Task<App?> GetAppFromDirAsync(string path)
    {
        return Task.FromResult(App.FromDirectory(path));
    }

    public async Task LaunchApp(App? app)
    {
        if (app == null)
        {
            Console.WriteLine("Error: app is null");
            return;
        }

        if (!app.HasAsset(AssetType.Script, "main"))
        {
            Console.WriteLine("Error: app has no main");
            return;
        }

        var state = LuaState.Create();
        Storage.Init(app);
        Permissions.Init(app);
        LuaImplementation.Init(state, app);
        Asset mainScript = app.GetAsset(AssetType.Script, "main")!;

        await state.DoStringAsync(Encoding.UTF8.GetString(mainScript.Data));
    }
}