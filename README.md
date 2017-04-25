# dnpatch
[WIP] .NET Patcher library using dnlib.

*If you have questions feel free to ask me via Gitter! I'm glad to help you out! Taking feature requests!*

[![Build status](https://ci.appveyor.com/api/projects/status/39jhu0noimfkgfw2?svg=true)](https://ci.appveyor.com/project/ioncodes/dnpatch)
[![Github All Releases](https://img.shields.io/github/downloads/ioncodes/dnpatch/total.svg)](https://github.com/ioncodes/dnpatch/releases)
[![Join the chat at https://gitter.im/dnpatch/Lobby](https://badges.gitter.im/dnpatch/Lobby.svg)](https://gitter.im/dnpatch/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## What is dnpatch?
dnpatch is the ultimate library for all your .NET patching needs. It offers automated assembly patching, signature scanning and last but but not least bypassing of obfuscators by it's ability to find methods in renamed/obfuscated types. Since the stars on GitHub exploded in a few days, dnpatch has been extended by a couple of projects. The most important one is dnpatch.deobfuscation which integrates de4dot directly into dnpatch. Also there is dnpatch.script, which gives you the ability to write patchers with pure JSON!
The library itself, uses dnlib (see next part).

## Notes
Since dnpatch uses dnlib, it is highly recommended to use dnSpy to analyze your assemblies first, so it is guaranteed that you will use the correct names, offsets, etc, because it does use dnlib aswell.

## Recommendations
It is highly recommended to calculate the position of instructions instead of defining indexes, to ensure that the patcher will still work after assembly updates.

## Patching
The constructor takes the filename of the assembly.
```cs
Patcher patcher = new Patcher("Test.exe");
```
If you want to keep the old maxstack (for example for obfuscated assemblies) use the overload:
```cs
Patcher patcher = new Patcher("Test.exe", true);
```

### Targeting Methods
All methods take an object called Target as argument. The object is defined as follows:
```cs
public string Namespace { get; set; } // needed
public string Class { get; set; } // needed
public string Method { get; set; } // needed

/* If you want to patch multiple indexes in the method */
public int[] Indexes { get; set; }
public Instruction[] Instructions { get; set; }

/* If you want to patch 1 index in the method */
public int Index { get; set; } = -1;
public Instruction Instruction { get; set; }

/* If the path to the method has more than 1 nested class use this */
public string[] NestedClasses { get; set; }

/* If the path to the method has 1 nested class use this */
public string NestedClass { get; set; }

/* If you want to set the parameters for the method (if it's overloaded) use this */
public string[] Parameters { get; set; }

/* If you want to set the return type for the method use this */
public string ReturnType { get; set; }

/* If you want to rewrite the getters or setters of a property use this */
public string Property { get; set; } // The name
public PropertyMethod PropertyMethod { get; set; } // See below, determines patch target
```
ReturnType and Parameters are case sensitive!
Example:
* String[]
* Int32
* etc

PropertyMethod is defined as this:
```cs
public enum PropertyMethod
{
	Get,
	Set
}
```

Please make sure that you don't assign inconsistent values, e.g.
```cs
var target = new Target
{
    Instructions = ...
    Instruction = ...
}
```

If you want to patch multiple methods create a Target[] and pass it to the functions, it is accepted by the most of them.

### Creating Instructions
Reference dnlib and create an Instruction[] or Instruction with your Instruction(s) and assign Indexes (int[]) or Index with the indexes where the Instructions are. You can find them by reverse engineering your assembly via dnSpy or some other decompiler.

Small Example:
```cs
Instruction[] opCodes = {
    Instruction.Create(OpCodes.Ldstr, "Hello Sir 1"),
    Instruction.Create(OpCodes.Ldstr, "Hello Sir 2")
};
int[] indexes = {
    0, // index of Instruction
    2
};
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "Print",
    Instructions = opCodes,
    Indexes = indexes
};
```

### Patch the whole methodbody
To clear the whole methodbody and write your instructions, make sure that you don't assign the Indexes or Index property.

Here is an example:
```cs
Instruction[] opCodes = {
    Instruction.Create(OpCodes.Ldstr, "Hello Sir"), // String to print
    Instruction.Create(OpCodes.Call, p.BuildMemberRef("System", "Console", "WriteLine")), // Console.WriteLine call -> BUILDMEMBERREF IS ONLY FOR CONSOLE.WRITELINE
    Instruction.Create(OpCodes.Ret) // Alaway return smth
};
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "Print",
    Instructions = opCodes
};
```

### Apply the patch
To apply your modified instructions you can call the method 'Patch':
```cs
patcher.Patch(Target);
```
or
```cs
patcher.Patch(Target[]);
```

### Finding an instruction
In some cases it might be useful to have find an instruction within a method, for example if the method got updated.
```cs
Instruction opCode = Instruction.Create(OpCodes.Ldstr, "TheTrain");
Instruction toFind = Instruction.Create(OpCodes.Ldstr, "TheWord");
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "FindMe",
    Instruction = opCode // you can also set it later
};
target.Index = p.FindInstruction(target, toFind);
// now you have the full Target object
```

Let's say there are multiple identical instructions. What now, baoss? Well, it's simple. There's an overload that takes and int which is the occurence of the instruction which you'd like to find.
```cs
Instruction opCode = Instruction.Create(OpCodes.Ldstr, "TheTrain");
Instruction toFind = Instruction.Create(OpCodes.Ldstr, "TheWord");
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "FindMe",
    Instruction = opCode // you can also set it later
};
target.Index = p.FindInstruction(target, toFind, 2); // Sir, find the second occurence!
```

### Finding methods by OpCode signature
You can find methods (Target[]) by scanning their body for an OpCode signature
```cs
OpCode[] codes = new OpCode[] {
	OpCodes.Ldstr,
	OpCodes.Call
};
var result = p.FindMethodsByOpCodeSignature(codes); // holds Target[]
```

### Replacing instructions
In some cases it might be easier to just replace an instruction. At this point of development, it doesn't make much sense, but the features will come soon.
```cs
Instruction opCode = Instruction.Create(OpCodes.Ldstr, "I love kittens");
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "ReplaceMe",
    Instruction = opCode,
    Index = 0
};
p.ReplaceInstruction(target);
```

### Removing instructions
Let's say you want to remove instructions... Well it's simple as this:
```cs
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "RemoveMe",
    Indexes = new[]{0,1} // the indexes, you can also just use 'Index'
};
p.RemoveInstruction(target);
```

### Patching operands
Hmmm.... What if you find the console output offending? You can modify the Ldstr without even creating an instruction :)
```cs
Target target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "PrintAlot",
    Index = 0
};
p.PatchOperand(target, "PatchedOperand"); // pass the Target and a string to replace
```
or incase you need to modify an int:
```cs
p.PatchOperand(target, 1337);
```
It is also able to patch multiple operands in the same method by using int[] or string[].

### Returning true/false
If you want to overwrite the methodbody with a return true/false statement you can do this:
```cs
target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "VerifyMe"
};
p.WriteReturnBody(target, bool); // bool represents the return value
```

### Clearing methodbodies
If you just want to empty a methodbody, use this amigo:
```cs
target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "WriteLog"
};
p.WriteEmptyBody(target);
```

### Getting instructions from target
Simply do this if you want to get instructions of the Target object:
```cs
target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "WriteLog"
};
Instruction[] instructions = p.GetInstructions(target);
```

### Writing return bodies
If you want to overwrite the body with a return true/false do this:
```cs
target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "WriteLog"
};
p.WriteReturnBody(target, bool);
// bool is the return value, e.g. true will return true ;)
```
If you want to remove the body simply call this:
```cs
target = new Target()
{
    Namespace = "Test",
    Class = "Program",
    Method = "WriteLog"
};
p.WriteEmptyBody(target);
```

### Find methods
If you want to find a method, you can simply scan the whole file by 2 ways:
```cs
p.FindInstructionsByOperand(string[]);
// or p.FindInstructionsByOperand(int[]);
// string[] with all operands in the method, if there are multiple identical operands, make sure to have the same amount as in the method.

// or do this via opcodes:
p.FindInstructionsByOpcode(OpCode[]);
```
Both ways return an Target[] which contains all targets pointing to the findings.

#### Find instructions in methods or classes
If you want to find the instructions and you know the class and optionally the method you can let this method return a Target[] with the pathes and indexes.
```cs
p.FindInstructionsByOperand(Target,int[],bool);
// int[]: the operands
// bool: if true it will search for the operands once, it will delete the index if the index was found

// for opcodes:
p.FindInstructionsByOpcode(Target,int[],bool);
```

### Patch properties
Now you can rewrite a propertie's getter and setter like this:
```cs
target = new Target()
{
	Namespace = "Test",
	Class = "Program",
	Property = "IsPremium", // Property name
	PropertyMethod = PropertyMethod.Get, // Getter or Setter
	Instructions = new []
	{
		Instruction.Create(OpCodes.Ldc_I4_1),
		Instruction.Create(OpCodes.Ret)  
	} // the new instructions
};
p.RewriteProperty(target); // Will overwrite it with return true in getter
```
The property called 'Property' holds the name of the target property.  
PropertyMethod can be 'PropertyMethod.Get' or 'PropertyMethod.Set'.  
Instructions are the new Instructions for the getter or setter.

### Building calls
To build calls like "Console.WriteLine()" you can use this method:
```cs
p.BuildMemberRef(string, string, string, Patcher.MemberRefType);
/* 
 * string 1 -> namespace, e.g. "System"
 * string 2 -> class, e.g. "Console"
 * string 3 -> method, e.g. "WriteLine"
 * MemberRefType -> the reference type, e.g. Static
 */
```
MemberRefType is defined as follows:
```cs
public enum MemberRefType
{
    Static,
    Instance
}
```
Here is an IL example for Console.WriteLine:
```cs
Instruction.Create(OpCodes.Call, p.BuildMemberRef("System", "Console", "WriteLine", Patcher.MemberRefType.Static));
```

### Injecting methods (Untested)
If you want to inject methods into classes, call InjectMethod. Make sure to set MethodDef and Instructions. Optionally set Locals, ParameterDefs.
```cs
Target target = new Target();
MethodImplAttributes methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
MethodAttributes methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
MethodDef meth1 = new MethodDefUser("MyMethod",
            MethodSig.CreateStatic(mod.CorLibTypes.Int32, mod.CorLibTypes.Int32, mod.CorLibTypes.Int32),
            methImplFlags, methFlags);
target.ParameterDefs = new[] { new ParamDefUser("a", 1) };
target.Locals = new[] { new Local(mod.CorLibTypes.Int32) };
target.MethodDef = meth1;
target.Class = "";
// ... target as always...
patcher.InjectMethod(target);
```
For now refer to this page: https://github.com/0xd4d/dnlib/blob/master/Examples/Example2.cs

### Saving the patched assembly
If you want to safe the assembly under a different name use this:
```cs
patcher.Save(String); // filename here
```
Or if you want to replace the original file:
```cs
patcher.Save(bool); // if true it will create a backup first (filename.bak)
```

## Deobfuscation [BETA]
Baoss, what can I do if it's heavily obfuscated?! Well, listen careful to your grandpa Joe. Use 'dnpatch.deobfuscation'! It has magic powers! Nah, Joe is just kiddin', it uses the de4dot libraries.
Reference the library dnpatch.deobfuscation and make sure that you also copy all others from the zip!
Then do this:
```cs
Deobfuscation d = new Deobfuscation(string, string);
// string 1 -> file to deobfuscate
// string 2 -> new filename for the deobfuscated file
d.Deobfuscate(); // Deobfuscates the file and writes it to the disk
```

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

# Credits
I'd like to thank these people:
* [0xd4d](https://github.com/0xd4d) for creating [dnlib](https://github.com/0xd4d/dnlib)
* [0xd4d](https://github.com/0xd4d) for creating [de4dot](https://github.com/0xd4d/de4dot)
* [Rottweiler](https://github.com/Rottweiler) for the PRs and help!
* [0megaD](https://github.com/0megaD) for the fixes which my eyes missed and for using dnpatch in his projects!