# Event Api

Call subscribed functions from anywhere in your code by triggering event. This API makes developing apps way easier.

### Events?
Events, yes. Events can be `created`, `subscribed` to, `unsubscribed` from and `called` (with arguments, optionally). Basically, if you created an event in one script, and subscribed to the event in another script (**NOTE!** You should [call the script](https://github.com/Ict00/TtyPack/blob/master/docs/apis/script_api.md) in which you subscribe functions to the event, otherwise it WON'T work.) then you'll be able to call all the functions subscribed from any place by calling the event. **EVENTS ARE GLOBAL FOR THE APP**.

### Functions
> `event.create(string event_id) -> nil`
> - Create an event with id.

> `event.subscribe(string event_id, function) -> nil`
> * Subscribe a function to the event.

> `event.unsubscribe(string event_id, function) -> bool`
> * Unsubscribe a function from the event. Returns `false` if event doesn't exist OR the function wasn't subscribed, otherwise - `true`.

> `event.call(string event_id, argument1, argument2...) -> nil`
> * Call all the subscribed functions with arguments (which are optional!)

### Usage example
`idmap.json`
```json
{
  "main": "src/main.lua",
  "other": "src/other.lua"
}
```

`src/other.lua`
```lua
function b(msg)
	console.println("Message received by other: ", msg)
end

event.subscribe("some_event", b)
```

`src/main.lua`
```lua
function a (msg)
	console.println("Message received by main: ", msg)
end

event.create("some_event")
event.subscribe("some_event", a)
script.call("other") -- Call 'other' script; You can read more about it in Script API documentation
event.call("some_event", "Same message!")
```

Output should be:
```
Message received by main: Same message!
Message received by other: Same message!
```
