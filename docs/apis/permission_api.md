# Permissions Api

Remember `Android` permissions? These are somehow similar. Permission API allows you to add your own permissions or ask for permission to execute sensitive code (for example, most of [System API](https://github.com/Ict00/TtyPack/blob/master/docs/apis/system_api.md) requires permissions)

### Permissions?
Basically, permissions have their own unique `id`, can't be overriden manually without asking user. Permission can be `denied`, _`allowed once`_ or `allowed always`. Permissions are stored in `.ttystorage/{app name}/permissions`. It's important to note that `permission.ask()` and `permission.request()` are different, since `permission.ask()` asks user AND returns the result (which is sensitive for other APIs permissions since once the result is checked, `allowed once` turns into `denied`) so **it's recommended to use permission.ask() for custom permissions and permission.request() for APIs permissions**
> **IMPORTANT NOTE: APIs WON'T ASK FOR PERMISSION WHEN FUNCTION IS CALLED, YOU SHOULD DO IT YOURSELF**

### Functions
> `permission.add(string permission_id) -> nil`
> - Adds permission with specified id, if it wasn't added before.

> `permission.has(string permission_id) -> bool`
> * Checks if permission was granted or not. If permission was `allowed once` before checking, it'll turn into `denied` after the check.

> `permission.request(string permission_id, string? reason = nil) -> nil`
> * Ask for permission from user.

> `permission.ask(string permission_id, string? reason = nil) -> bool`
> * Basically `permission.request()` + `permission.has()` combined

### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua"
}
```

`src/main.lua`
```lua
permission.add("my_permission")

function plead()
	if permission.has("my_permission") then
		console.println("Nevermind :3")
	else
		permission.request("my_permission", "pretty please?")
		plead()
	end	
end

plead()
```

This app will ask you for permission till you give it. Only then it will print `Nevermind :3` in console.
