using Lua;
using MessagePack;
using TtyPack.Bridges;

namespace TtyPack;

[MessagePackObject]
[Union(0, typeof(DataTable))]
[Union(1, typeof(DataList))]
[Union(2, typeof(DataNumber))]
[Union(3, typeof(DataBool))]
[Union(4, typeof(DataString))]
[Union(5, typeof(DataBytes))]
[Union(6, typeof(DataNil))]
public abstract class DataUnit
{
    public LuaValue TryRead()
    {
        if (this is DataNumber number)
        {
            return number.Value;
        }

        if (this is DataTable table)
        {
            return table.FromData();
        }

        if (this is DataList list)
        {
            return list.FromDataList();
        }

        if (this is DataBool dataBool)
        {
            return dataBool.Value;
        }

        if (this is DataString dataString)
        {
            return dataString.Value;
        }

        if (this is DataBytes dataBytes)
        {
            return LuaValue.FromObject(dataBytes.Value);
        }

        return LuaValue.Nil;
    }
}

[MessagePackObject]
public class DataTable(Dictionary<string, DataUnit> data) : DataUnit
{
    [Key(0)] public Dictionary<string, DataUnit> DataUnits { get; } = data;
}

[MessagePackObject]
public class DataList(List<DataUnit> units) : DataUnit
{
    [Key(0)] public List<DataUnit> DataUnits { get; } = units;
}

[MessagePackObject]
public class DataNumber(double value) : DataUnit
{
    [Key(0)] public double Value { get; } = value;
}

[MessagePackObject]
public class DataBool(bool value) : DataUnit
{
    [Key(0)] public bool Value { get; } = value;
}

[MessagePackObject]
public class DataString(string value) : DataUnit
{
    [Key(0)] public string Value { get; } = value;
}

[MessagePackObject]
public class DataBytes(byte[] value) : DataUnit
{
    [Key(0)] public byte[] Value { get; } = value;
}

[MessagePackObject]
public class DataNil : DataUnit;

