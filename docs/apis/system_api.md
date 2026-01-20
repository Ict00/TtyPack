
# System Api

Whenever you need for your app to be `out-of-box`, you'll have to use this API. It includes interaction with filesystem and some information about system. Also, this `API` requires [Permissions](https://github.com/Ict00/TtyPack/blob/master/docs/apis/permission_api.md) which you need to request first before doing most operations.

### Functions
> `system.read_dir(string path, string? filter = "*", bool? recursive = false) -> table`
> - Read the dir. Requires `fs.read` permission. Returns array of all listed entries.

> `system.file_exists(string path) -> bool`
> * Check if file exists. Requires `fs.read` permission.

> `system.dir_exists(string path) -> bool`
> * Check if directory exists. Requires `fs.read` permission.

> `system.read_file_str(string path) -> string`
> * Returns file's contents. Requires `fs.read` permission. Returns empty `string` if error occurred during reading.

> `system.read_file_bytes(string path) -> byte[]`
> * Returns file's contents. Requires `fs.read` permission. Returns empty `byte[]` if error occurred during reading.

> `system.write_file_str(string path, string content) -> nil`
> * Write file's contents. Requires `fs.write` permission. Returns empty `string` if error occurred during writing.

> `system.write_file_bytes(string path, byte[] content) -> nil`
> * Write file's contents. Requires `fs.write` permission. Returns empty `byte[]` if error occurred during writing.

> `system.get_argv() -> table`
> * Get all the `commandline args` given to the _app_ (**NOT TTYPACK AS WELL**! if you prompted: `TtyPack run-file some.ttypack arg1 arg2`, there will be only { "arg1", "arg2" } returned!). Requires `os.argv` permission.

> `system.get_username() -> string`
> * Get current user's name system. Requires `os.identity` permission.

> `system.os_run(string cmd) -> nil`
> * Executes specified Cmd. **BE VERY CAREFUL WITH THIS, AS USING THIS INCORRECTLY MAY LEAD TO BYPASSING OTHER PERMISSIONS, WHICH IS UNRECOMMENDED**. Requires `os.run` permission.

> `system.get_current_dir() -> string`
> * Get current directory (the entry from where the command was executed). Requires `os.identity` permission.

> `system.combine_paths(path1, path2...) -> string`
> * Combine all specified paths (`string`s) in crossplatform-friendly way. Doesn't require any permission.

> `system.time()`
> * Corresponds to `Lua`'s `os.time`. Doesn't require any permission.

> `system.date()`
> * Corresponds to `Lua`'s `os.date`. Doesn't require any permission.

### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua",
  "text": "src/funny_text.txt"
}
```

`src/funny_text.txt`
```
Fun :D
```

`src/main.lua`
```lua
local text = assets.get("text").data
permission.request("fs.write")
system.write_file_str("cool_file.txt", text)
```

If everything is done correctly, `cool_file.txt` with `Fun :D` should appear.
