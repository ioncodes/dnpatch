# dnpatch
.NET Patcher library using dnlib.

[![Build Status](https://travis-ci.org/ioncodes/dnpatch.svg?branch=master)](https://travis-ci.org/ioncodes/dnpatch)

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
```
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

........ Gimme a few days
