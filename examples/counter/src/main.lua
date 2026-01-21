local count = storage.get_number("counter")
if count == 0 then count = 1 end

console.println("Application ran #", count, " time")
count = count + 1
storage.put("counter", count)
storage.commit()