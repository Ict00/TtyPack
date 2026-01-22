# Script Api

The most simple API, yet useful. It lets you use other scripts you have and borrow their functions/variables >:). Mostly suited for cross-script communication.

### How it works?
Every script is being run in a different `LuaState`, which means that it can't access other scripts' variables and functions (without borrowing).
#### Variable borrowing
First time script gets executed (except for `script.run("...")` as it has only body, no name), its functions and variables (general state) goes to `borrow table` (or `borrow map`, name it any way you want). When borrowing (`script.borrow("script", "var_or_function_name")`), `TtyPack` checks if the script is present in `borrow table`. If yes, it'll try to return the requested variable/function (resulting nil if there is no such), otherwise it will execute the script first and only then borrow the function/variable, returning it.
**NOTE: Usage of Script API may be bad for the performance as it's not polished yet**

### Functions
> `script.run(string script_body) -> ...`
> - Executes specified `string` in separate `LuaState`. Returns whatever script returns.

> `script.call(string script_id) -> ...`
> * Executes specified script (AND ASSIGNS IT TO `Borrow Table`). Returns whatever script returns.

> `script.borrow(string script_id, string var_or_function_name) -> ...`
> * Borrow the variable/function of specified script

> `script.import(string script_id) -> nil`
> * Imports the script to current scope (without overriding existing variables)

> `script.try_catch(function try_function, function? catch_function) -> nil/...`
> * Tries to execute the `try_function` and return the values it returns (`try-function` should NOT have any parameters).
> * If exception was thrown during execution, `catch_function` would be executed and `nil` would be returned;
> * Passing `catch_function` is optional, but if you do, it should have only ONE parameter, `exception`:
> * ```
>   {
>       message = string,
>       source = string,
>       traceback = string
>   }
>   ```


### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua",
  "other": "src/other.lua"
}
```

`src/other.lua`
```
console.println("other.lua is being executed")

function getmsg(msg)
	console.println("Message: ", msg)
end

function sum(a, b)
	return a + b
end
```

`src/main.lua`
```lua
local f = script.borrow("other", "getmsg")
f("Hey!")
local g = script.borrow("other", "sum")
console.println(g(1, 9))
```

Output should be:
```
other.lua is being executed
Message: Hey!
10
```
