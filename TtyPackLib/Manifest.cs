using Newtonsoft.Json;

namespace TtyPackLib;

[method: JsonConstructor]
public class Manifest(string name, string author, string description)
{
    public string Name { get; set; } = name;
    public string Author { get; set; } = author;
    public string Description { get; set; } = description;
}