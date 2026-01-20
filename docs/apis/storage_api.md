# Storage API

Using this API is the best way to store persistent data of your app which doesn't require any permission unlike [System API](https://github.com/Ict00/TtyPack/edit/master/docs/apis/system_api.md)

### Storage?
Basically, every app has its own directory where all the artifacts are stored: `.ttystorage/{app name}/artifact`. One of those artifacts is `localstorage`. It lets you save the data between sessions without affecting `OS` filesystem much. Everytime the change is done to storage, you should do `storage.commit()` to save it (though, if the change is done often, it's better to call `storage.commit()` less frequently). **STORAGE API WON'T SAVE THE DATA FOR YOU**. Also, only these types of objects can be saved: `int`, `float`, `bool`, `string`, `LuaTable*`, `byte[]` and `nil`. `LuaTable`s are special case since they can't be directly serialized. Instead, they are parsed into `DataTables`.
### What are DataTables?
`DataTable` is simplified representation of `LuaTable` which is easier to serialize/deserialize. Functions and recursive tables **ARE NOT SUPPORTED** and will be replaced with `nil`, mixed tables (like `{ 1, 2, x=10, 3}`) are not supported as well but only array part of table will be parsed.

**ALSO NOTE: IF THE `GET` OPERATION WASN'T SUCCESSFUL, ALL `storage.get_X()` FUNCTIONS WILL RETURN DEFAULT VALUE, NOT NIL!** 
### Functions
> `storage.commit() -> bool`
> - Saves all the changes done. Returns `false` if the save was unsuccessful.

> `storage.clear_all() -> nil`
> * Clears all the data of the app. **BE CAREFUL WITH THIS ONE!**

> `storage.put(string id, object) -> nil`
> * Puts the object (of any type that was specified above) into the storage by specified `id`.

> `storage.remove(string id) -> bool`
> * Remove the object by `id`. Returns `true` if the operation was successful.

> `storage.has(string id) -> bool`
> * Checks if the `id` is in the storage

> `storage.get_int(string id) -> int`
> * Tries to get the object by `id`

> `storage.get_float(string id) -> float`
> * Tries to get the object by `id`

> `storage.get_bytes(string id) -> byte[]`
> * Tries to get the object by `id`

> `storage.get_bool(string id) -> bool`
> * Tries to get the object by `id`

> `storage.get_string(string id) -> string`
> * Tries to get the object by `id`

> `storage.get_table(string id) -> table`
> * Tries to get the object by `id`

### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua"
}
```

`src/main.lua`
```lua
if storage.has("name") then
	local name = storage.get_string("name")
	console.println("Hey there, ", name, "!")
else
	console.print("What's your name? > ")
	local name = console.readln()
	console.println("Alright, I'll remember it for the next time")
	storage.put("name", name)
	storage.commit() -- DON'T FORGET
end
```
