# TtyPack: Single-file Lua apps for the terminal.
### Under construction 🏗️
TtyPack lets you create self-contained terminal applications using Lua.
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
