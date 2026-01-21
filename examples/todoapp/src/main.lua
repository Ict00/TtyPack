function set_cursor(x, y)
    console.print("\27[", y, ";", x, "H") -- More info about ANSI Escape codes: https://gist.github.com/fnky/458719343aabd01cfb17a3a4f7296797
end

-- Task:
-- {
--     date = time,
--     name = string,
--     completed = bool
-- }

tasks = storage.get_table("tasks")
screen = {
    width = console.get_width(),
    height = console.get_height()
}

function make_task_now(name)
    return {
        date = system.date("*t"),
        name = name,
        completed = false
    }
end

function save()
    storage.put("tasks", tasks)
    storage.commit()
end

function output_task(task, i, longest)
    local whenCompleted = " "
    if task.completed then whenCompleted = "x" end
    if longest == nil then longest = math.floor(screen.width/3) end
    local format = string.format("%%-%ds - %%s", longest)
    local date = task.date
    console.println(i, " =[", whenCompleted, "] ", string.format(format, task.name, string.format("%d-%02d-%02d %02d:%02d:%02d", date.year, date.month, date.day, date.hour, date.min, date.sec)))
end

function draw_app()
    console.clear()
    set_cursor(0, 0)
    console.println("TODO APP")

    for i, t in ipairs(tasks) do
        output_task(t, i)
    end
end

function draw_footer()
    set_cursor(0, screen.height-1)
    console.print("[A] Add | [C] Set completed | [R] Remove | [Q] Quit")
end

console.print("\27[?25l")

while true do
    draw_app()
    draw_footer()
    local input = console.readkey(true)

    -- If key is 'Q'
    if input.key == 81 then
        break
    end

    -- If key is 'A'
    if input.key == 65 then
        console.clear()
        set_cursor(0, 0)
        console.print("\27[?25hAdd a task > ")
        local newTaskInput = console.readln()
        if #newTaskInput > 0 then
            table.insert(tasks, make_task_now(newTaskInput))
            save()
        end
        console.print("\27[?25l")
    end
    
    -- If key is 'R'
    if input.key == 82 then
        console.print("\27[2K\27[", screen.width, "D")
        console.print("Select the task you want to remove > ")
        local selected = console.readln()
        local id = tonumber(selected)
        if id ~= nil then
            if id > 0 and id <= #tasks then
                table.remove(tasks, id)
                save()
            end
        end
    end

    -- If key is 'C'
    if input.key == 67 then
        console.print("\27[2K\27[", screen.width, "D")
        console.print("Select the task you want to mark as completed > ")
        local selected = console.readln()
        local id = tonumber(selected)
        if id ~= nil then
            if id > 0 and id <= #tasks then
                tasks[id].completed = true
                save()
            end
        end
    end
end

console.print("\27[?25h")
console.clear()