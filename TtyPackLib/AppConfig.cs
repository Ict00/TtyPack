using Newtonsoft.Json;

namespace TtyPackLib;

public static class AppConfig
{
    public static Dictionary<string, string> GetIdMap(this string config)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(config)??[];
    }
}