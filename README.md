# dnpatch
[WIP] .NET Patcher library using dnlib.

*If you have questions feel free to ask me via Gitter! I'm glad to help you out!*

[![Build status](https://ci.appveyor.com/api/projects/status/39jhu0noimfkgfw2?svg=true)](https://ci.appveyor.com/project/ioncodes/dnpatch)
[![Github All Releases](https://img.shields.io/github/downloads/ioncodes/dnpatch/total.svg)]
(https://github.com/ioncodes/dnpatch/releases)
[![Join the chat at https://gitter.im/dnpatch/Lobby](https://badges.gitter.im/dnpatch/Lobby.svg)](https://gitter.im/dnpatch/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Patching
The constructor takes the filename of the assembly.
```cs
Patcher patcher = new Patcher("Test.exe");
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

### Saving the patched assembly
If you want to safe the assembly under a different name use this:
```cs
patcher.Save(String); // filename here
```
Or if you want to replace the original file:
```cs
patcher.Save(bool); // if true it will create a backup first (filename.bak)
```

## Obfuscated assemblies...
This part is in heavy development right now. The main purpose is to find a method and patch the instructions without the need of knowing the names incase the namespaces, etc are renamed via an obfuscator.

### Constructor
Create an Object called 'ObfuscationPatcher':
```cs
var op = new ObfuscationPatcher(string, bool);
// string: filename
// bool: keep old max stack?
```

### Searching the target method
#### By operand
Let's say the strings are not encrypted (I'm talking about Ldstr here):
```cs
string[] operands = {
    "Find",
    "TheWord",
    "The",
    "Word",
    "You",
    "Wont"
}; // Find there words within the method.
Target[] obfuscatedTargets = op.FindInstructionsByOperand(operands); // let the boi work. It will return an Target[] with all methods he could find.
foreach (var obfTarget in obfuscatedTargets) // Let's iterate and have fun
{
    obfTarget.Instructions = new Instruction[]
    {
        Instruction.Create(OpCodes.Ldstr, "Obfuscator"),
        Instruction.Create(OpCodes.Ldstr, "Got"),
        Instruction.Create(OpCodes.Ldstr, "Rekt"),
        Instruction.Create(OpCodes.Ldstr, "Hell"),
        Instruction.Create(OpCodes.Ldstr, "Yeah"),
        Instruction.Create(OpCodes.Ldstr, "!")
    }; // modify the instructions
}
op.Patch(obfuscatedTargets); // Patch the instructions
```

#### By OpCode
Let's say you look for an OpCode, you can do this:
```cs
op.FindInstructionsByOpcode(new[] {OpCodes.Add}) // NOT TESTED
```

### Patching
As before you will just do this:
```cs
op.Patch(Target); // Patch the instructions
// or
op.Patch(Target[]); // Patch multiple targets
```

### Save the assembly
Again, as before :)
```cs
op.Save(string); // string -> filename
```

## Resources
Wanna patch some resources? No, problem! Just create an object called ResourcePatcher!
```cs
ResourcePatcher rp = new ResourcePatcher(string); // string-> assembly
```

### Insert resources
Add resources using this piece of code:
```cs
rp.InsertResource(string, byte[]);
/*
 * string -> resourcename
 * byte[] -> ByteArray of the data to write
 */
```
You can replace byte[] with a string to load the byte[] from a file.

### Removing resources
You can remove resources if you know the index:
```cs
rp.RemoveResource(int); // int -> index
```
If you want to remove all resources do this:
```cs
rp.RemoveResources();
```

### Replacing resources
If you want to replace a resource, you can do this my friend:
```cs
rp.ReplaceResource(int, string, byte[]);
/*
 * int -> index
 * string -> name
 * byte[] -> ByteArray with your data
 */
```
You can replace byte[] with a string pointing to a file.

### Getting resources
dnlib stores the resources as ResourceCollection and I'm not going to wrap it, use this method to get the resources:
```cs
rp.GetResources();
```

### Save
As always:
```cs
rp.Save(string);
// or
rp.Save(bool);
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

# Credits
I'd like to thank these people:
* [0xd4d](https://github.com/0xd4d) for creating [dnlib](https://github.com/0xd4d/dnlib)
* [0xd4d](https://github.com/0xd4d) for creating [de4dot](https://github.com/0xd4d/de4dot)
