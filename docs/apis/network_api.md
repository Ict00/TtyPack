# Network Api

Use the network. Also, see the [Json Module](https://github.com/Ict00/TtyPack/blob/master/docs/apis/json_api.md) 
> NOTE: This module is in development, it'll have way more functions in soon updates (like http listeners, tcp client/listeners and so on)

> Response table:
> ```
> {
>     status = number,
>     body = string
> }
> ```

### Functions
> `network.http.get(string uri) -> response_table/nil`
> - Send a GET request. Returns nil if failed to send request

> `network.http.post(string uri, table/string content, string? content_type) -> response_table/nil`
> * Send a POST request. If content is `table`, it'll be automatically serialized into JSON (and `content_type` will be
> * `application/json`).


### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua"
}
```


`src/main.lua`
```lua
local x = network.http.get("https://archlinux.org/packages/core/x86_64/coreutils/files/json/")
local y = json.deserialize(x)
console.println(y.pkgname, " - ", y.repo, " - ", y.arch)
```

Output should be:
```
coreutils - core - x86_64
```
