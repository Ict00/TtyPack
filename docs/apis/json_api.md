
# Json API

This API uses `DataTable`s described in [Storage API](https://github.com/Ict00/TtyPack/blob/master/docs/apis/storage_api.md)
 to serialize/deserialize tables into string.

### Functions
> `json.serialize(table) -> string`
> - Serialize table into json

> `json.serialize(string) -> table/number/string/nil/bool`
> - Deserialize any value from json string


### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua"
}
```

`src/main.lua`
```lua
local example_table = { a = 1, b = 2, c = {1,2,3,4,5}, e = false}
local table_string = json.serialize(example_table)
console.println(table_string)
```

Output should be:
```
{"a":1.0,"b":2.0,"c":[1.0,2.0,3.0,4.0,5.0],"e":false}
```
