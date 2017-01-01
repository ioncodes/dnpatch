## Scripting
With dnpatch.script you're now able to script patchers with JSON!
Example JSON:
```json
{
    "target":"Test.exe",
    "targets":[{
        "namespace":"Test",
        "class":"Program",
        "method":"ReplaceMe",
        "action":"replace",
        "index":0,
        "instructions":[{
            "opcode":"ldstr",
            "operand":"script working"
        }]
    },{
        "namespace":"Test",
        "class":"Program",
        "method":"RemoveMe",
        "action":"empty"
    }]
}
```
Name this file script.json and place it into TestScript build folder and use it with Test.exe. For more info please refer to the [standalone repo](https://github.com/ioncodes/dnpatch.script).
