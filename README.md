# TtyPack: Single-file Lua apps for the terminal.
### Under construction 🏗️
TtyPack lets you create self-contained terminal applications using Lua and C#-provided built-in modules. Think
of it as of custom standart library + bundling tool.
Package scripts, assets, and data into a single .ttypack file that runs anywhere with built-in secure sandboxing.

### Why TtyPack?

Not only TtyPack makes it easier to share your apps, but also it's more safe because of _Permissions_.
Also, using TtyPack you are given a lot of different APIs:

* Assets
* Console
* Events
* Permissions
* Script
* Storage
* System

So that development experience is way better.


### Limitations
Due to `Lua CSharp` limitations `C-based Lua` libraries are not supported.
Also, `Os`, `Io` and `Debug` libraries are disabled for sandboxing reasons. But `TtyPack` offers permission-based `System` module in exchange.
There are a lot of different modules (`API`s) planned:

* Net
* Json
* Serialization
* ...and more, if **you** suggest more

#### How to use it?

First of all, you would need to have `.NET 10` installed on your machine.
Then, clone the repository:
```shell
$ git clone https://github.com/Ict00/TtyPack.git
$ cd TtyPack/TtyPack
```
Then build it:
```shell
$ dotnet publish
```

That's all. The executable will be located at `bin/Release/net10.0/(your platform)/publish/TtyPack`

#### Supported platforms:
* [x] Linux
* [x] Windows (Not tested)
* [x] Mac OS (Not tested)

#### For developers:
[Main documentation](https://github.com/Ict00/TtyPack/blob/master/docs/main.md "Docs")

[Examples](https://github.com/Ict00/TtyPack/blob/master/examples "Examples")

If you encounter any bugs, report them in Github Issues. It'll help the project to improve <3
#### Important information for users:
TtyPack (**FOR NOW**) will create and use `.ttystorage` directory **IN THE SAME DIRECTORY AS EXECUTABLE** to store some data like
apps' local data and permissions. This will be changed in next updates.
