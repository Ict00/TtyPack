using MessagePack;

namespace TtyPack;

[MessagePackObject]
public class Permission(string id, PermissionLevel level)
{
    [Key(0)]
    public string Id { get; set; }= id;
    [Key(1)]
    public PermissionLevel Level { get; set; } = level;
}

public enum PermissionLevel
{
    Deny,
    AllowOnce,
    Allow
}