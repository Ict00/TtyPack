
# Byte Util Api

Byte util API is needed for conversions between C#'s `byte[]` data (which can't be handled by Lua directly) and Lua `table`s and `string`s.


### Important note
All the conversions between `string` and `byte` use **UTF-8** encoding. Keep it in mind.

### Functions
> `byteutil.bytes2str(byte[]) -> string`
> - Conversion from `byte[]` to `string` using `UTF-8`

> `byteutil.str2bytes(string) -> byte[]`
> - Conversion from `string` to `byte[]` using `UTF-8`

> `byteutil.bytes2table(byte[]) -> table`
> - Returns array representation of `byte[]`

> `byteutil.table2bytes(table) -> byte[]`
> - Inverse operation to `byteutil.bytes2table`.

### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua",
  "important": "src/important.bin"
}
```

`src/important.bin`
```
Blob :3
```

`src/main.lua`
```lua
local asset = assets.get("important") -- now data is byte[], since .bin is not textblob
console.println(byteutil.bytes2str(asset.data)) -- Blob :3
local table = byteutil.bytes2table(asset.data)

for _, c in ipairs(table) do
    console.println(c, " - ", string.char(c))
end 
```

Output should be:
```
Blob :3
66 - B
108 - l
111 - o
98 - b
32 -
58 - :
51 - 3
```
