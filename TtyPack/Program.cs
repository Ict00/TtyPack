using MessagePack;
using TtyPack;
using TtyPack.Modules;
using TtyPackLib;

if (args.Length == 0)
{
    Help();
}
var client = new Client();

var lst = args.ToList();

switch (args[0])
{
    case "run-file":
        if (args.Length < 2) Help();
        SystemModule.Args = lst.Slice(2, args.Length - 2).ToArray();
        
        var fileApp = await client.GetAppFromFileAsync(args[1]);
        await client.LaunchApp(fileApp);
        break;
    case "fetch":
        if (args.Length < 2) Help();
        SystemModule.Args = lst.Slice(2, args.Length - 2).ToArray();
        
        var fetchedApp = await client.GetAppFromUriAsync(args[1]);
        await client.LaunchApp(fetchedApp);
        break;
    case "run-dir":
        if (args.Length < 2) Help();
        SystemModule.Args = lst.Slice(2, args.Length - 2).ToArray();
        
        var dirApp = await client.GetAppFromDirAsync(args[1]);
        await client.LaunchApp(dirApp);
        break;
    case "pack":
        if (args.Length < 2) Help();
        var packApp = App.FromDirectory(args[1]);
        
        if (packApp == null)
        {
            Console.WriteLine("Error: app is null");
            return;
        }

        try
        {
            var app = MessagePackSerializer.Serialize(packApp);
            File.WriteAllBytes($"{packApp.Name}.ttypack", app);
        }
        catch(Exception e)
        {
            Console.WriteLine($"Unable to pack the app: {e.Message}");
        }
        
        break;
    case "help":
        Help();
        break;
    case "version":
        Console.WriteLine("TtyPack v0.2 [BETA]");
        break;
}

void Help()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("|- TtyPack help                                          - show this message");
    Console.WriteLine("|- TtyPack version                                       - TtyPack version");
    Console.WriteLine("|- TtyPack run-file <valid TtyPack app file>             - run the app from file");
    Console.WriteLine("|- TtyPack run-dir  <valid TtyPack app dir>              - run the app from directory");
    Console.WriteLine("|- TtyPack pack  <valid TtyPack app dir>                 - pack the app into file");
    Console.WriteLine("|- TtyPack fetch <URL that gives away valid TtyPack app> - run the app from directory");
    Environment.Exit(0);
}