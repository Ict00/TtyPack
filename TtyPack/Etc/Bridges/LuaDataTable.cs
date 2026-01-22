using Lua;

namespace TtyPack.Bridges;

public static class Lua2DataBridge
{
    private static List<LuaTable> knownTables = [];
    
    extension(LuaTable table)
    {
        public DataUnit FromLua()
        {
            var result = ParseAny(table);
            knownTables.Clear();
            return result;
        }

        private DataTable ParseTable()
        {
            knownTables.Add(table);
            var dataTable = new DataTable([]);

            foreach (var row in table)
            {
                var key = row.Key.Read<string>();
                var value = ParseAny(row.Value);
                dataTable.DataUnits[key] = value;
            }
        
            return dataTable;
        }
    }
    
    extension(LuaValue value)
    {
        public DataUnit ToDataUnit() => ParseAny(value);

        private DataUnit ParseAny()
        {
            if (value.TryRead(out LuaTable asTable) && !knownTables.Contains(asTable))
            {
                if (asTable.ArrayLength > 0) return ParseList(asTable);
                return ParseTable(asTable);
            }

            return ParseOtherUnit(value);
        }
    }

    private static DataList ParseList(this LuaTable table)
    {
        var dataList = new DataList([]);
        foreach (var row in table)
        {
            dataList.DataUnits.Add(ParseAny(row.Value));
        }
        return dataList;
    }

    private static DataUnit ParseOtherUnit(this LuaValue value)
    {
        if (value.TryRead<double>(out var number))
        {
            return new DataNumber(number);
        }

        if (value.TryRead<bool>(out var @bool))
        {
            return new DataBool(@bool);
        }

        if (value.TryRead<string>(out var @string))
        {
            return new DataString(@string);
        }

        if (value.TryRead<byte[]>(out var @bytes))
        {
            return new DataBytes(@bytes);
        }

        return new DataNil();
    }
}

public static class Data2LuaBridge {
    extension(DataTable table)
    {
        public LuaTable FromData()
        {
            return ParseTable(table);
        }

        private LuaTable ParseTable()
        {
            var luaTable = new LuaTable();
            
            foreach (var unit in table.DataUnits)
            {
                luaTable[unit.Key] = ParseAny(unit.Value);
            }
        
            return luaTable;
        }
    }

    extension(DataList list)
    {
        public LuaTable FromDataList()
        {
            return ParseList(list);
        }

        private LuaTable ParseList()
        {
            var luaTable = new LuaTable();

            int i = 1;
            foreach (var unit in list.DataUnits)
            {
                luaTable[i++] = ParseAny(unit);
            }
        
            return luaTable;
        }
    }

    private static LuaValue ParseAny(this DataUnit any)
    {
        if (any is DataList list) return ParseList(list);
        if (any is DataTable table) return ParseTable(table);
        if (any is DataBool dataBool) return dataBool.Value;
        if (any is DataString dataString) return dataString.Value;
        if (any is DataNumber dataNumber) return dataNumber.Value;
        if (any is DataBytes dataBytes) return LuaValue.FromObject(dataBytes.Value);
        
        return LuaValue.Nil;
    }
}