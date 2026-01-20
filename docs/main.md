# TtyPack documentation

## Quick Start
First of all, you should create empty directory. There two files are required: `manifest.json` that contains all the main information about your app and `idmap.json` that maps asset id to its location relatively to app.

`manifest.json`:
```
{
  "Name": "App Name",
  "Description": "Some Description",
  "Author": "You"
}
```

`idmap.json`:
```
{
  "main": "src/main.lua"
}
```
> NOTE: There always should be `main` asset that is script. Otherwise, the app won't be valid.

So, the most minimal app will have such structure:
```
App
|- manifest.json
|- idmap.json
|- src
   |- main.lua
```

### Scripting.

While most of Lua standart functionality is supported, some stuff like `require`, `os` and `io` was removed (for more "secure" execution). Thus, you will have to use `TtyPack`'s APIs:
* [Assets](https://github.com/Ict00/TtyPack/blob/master/docs/apis/asset_api.md)
* [Byte Util](https://github.com/Ict00/TtyPack/blob/master/docs/apis/byteutil_api.md)
* [Console](https://github.com/Ict00/TtyPack/blob/master/docs/apis/console_api.md)
* [Event](https://github.com/Ict00/TtyPack/blob/master/docs/apis/event_api.md)
* [Permissions](https://github.com/Ict00/TtyPack/blob/master/docs/apis/permission_api.md)
* [Storage](https://github.com/Ict00/TtyPack/blob/master/docs/apis/storage_api.md)
* [System](https://github.com/Ict00/TtyPack/blob/master/docs/apis/system_api.md)

> **`type?` notation means that the argument is optional**. It's important to understand everything described in the `API` docs.


So, `Hello, World!` app would look like this:

`src/main.lua`
```lua
console.println("Hello, World!")
```

### Packing.

Now that you made your first app, let's pack it! You'll have to use earlier compiled `TtyPack` executable. Do:
```shell
$ ./TtyPack pack path/to/app/dir
```

And then, if you did everything right, you'll get the `.ttypack` file, ready for execution. Alternatively, you can just run the app without packing it:
```shell
$ ./TtyPack run-dir path/to/app/dir
```
