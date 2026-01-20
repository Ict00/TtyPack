
# Console Api

You'll work with this API most of your time, probably.

### Functions
> `console.readkey(bool? intercept = false) -> table`
> - Read one single key pressed. If `intercept` is `true`, the key won't be displayed when pressed.
> - Returns such table:
> - ```lua
>   {
>       char = string,
>       control = bool,
>       alt = bool,
>       shift = bool,
>       key = int
>   }
>   ```
> - NOTE: `key` field corresponds to key number according to `C#`'s [ConsoleKey](https://learn.microsoft.com/en-us/dotnet/api/system.consolekey). 

> `console.println(object1, object2...) -> nil`
> * Print with `\n` at the end

> `console.print(object1, object2...) -> nil`
> * Print with no `\n` at the end

> `console.readln() -> string`
> * Reads user input

> `console.get_height() -> int`
> * Get buffer (terminal) height

> `console.get_width() -> int`
> * Get buffer (terminal) width

> `console.get_cursor() -> table`
> - Get cursor position (in terminal)
> - Returns such table:
> - ```lua
>   {
>       x = int,
>       y = int
>   }
>   ```

> `console.clear() -> nil`
> * Clear the screen
