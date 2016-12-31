## Scripting
With dnpatch.script you're now able to script patchers with JSON!
Example JSON:
```json
{
    "target":"Test.exe",
    "targets":[{
        "ns":"Test",
        "cl":"Program",
        "me":"ReplaceMe",
        "ac":"replace",
        "index":0,
        "instructions":[{
            "opcode":"ldstr",
            "operand":"script working"
        }]
    },{
        "ns":"Test",
        "cl":"Program",
        "me":"RemoveMe",
        "ac":"empty"
    }]
}
```
Name this file script.json and place it into TestScript build folder and use it with Test.exe. For more info please refer to the [standalone repo](https://github.com/ioncodes/dnpatch.script).