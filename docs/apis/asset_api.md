# Assets Api

It's one of the most important APIs in `TtyPack` that give you access to assets you added to `idmap.json` earier.

### Important note
There are two variants of asset representation. It's either `C#`'s `byte[]` or `string`, which depends on the way you get the asset AND the extension asset has. For example, `.txt`, `.lua`, `.json` files are parsed as `string`s by default (they are `textblob`s). But all the other formats are parsed just as `byte[]` (those are just `blob`s). To convert `byte[]` to `string` and vice versa use [Byte Util](https://github.com/Ict00/TtyPack/blob/master/docs/apis/byteutil_api.md).

### Functions
> `assets.get(string asset_id, bool? as_string = false) -> table`
> - Get the asset by `ID`. If asset is parsed as `string` by default, then `as_string` parameter won't affect it. Otherwise, it'll automatically convert `byte[]` to `string` (if `as_string = true`)
> - Returns such table:
> - ```lua
>   {
>       id = string,
>       type = "script"/"textblob"/"blob",
>       data = byte[]/string
>   }
>   ```

> `asset.has(string asset_id) -> bool`
> * Check if asset exists.

> `assets.push(string asset_id, byte[]/string asset_value) -> nil`
> * Add asset to **in-memory** asset list
