# dnpatch
[WIP] .NET Patcher library using dnlib.

*If you have questions feel free to ask me via Gitter! I'm glad to help you out! Taking feature requests!*

[![Join the chat at https://gitter.im/dnpatch/Lobby](https://badges.gitter.im/dnpatch/Lobby.svg)](https://gitter.im/dnpatch/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## What is dnpatch?
dnpatch is the ultimate library for all your .NET patching needs. It offers automated assembly patching, signature scanning and last but but not least bypassing of obfuscators by its ability to find methods in renamed/obfuscated types.  
The library itself uses dnlib (see next part).

## Notes
Since dnpatch uses dnlib, it is highly recommended to use dnSpy to analyze your assemblies first, to ensure that you use the correct names, offsets, etc, because it uses dnlib aswell.

## Recommendations
It is highly recommended that you calculate the instruction's index instead of defining it, to improve the likelihood of compatibility with future updates.

## 404
Looks like you found the v1 branch! Currently there's nothing to see here, but there will be one day, I promise 😉 You can actually help me, so **smash** the Gitter-badge and hit me up with some ideas, or send me pull requests.  
Let's make dnpatch **greater** *again* 🔥

# Credits
I'd like to thank these people:
* [0xd4d](https://github.com/0xd4d) for creating [dnlib](https://github.com/0xd4d/dnlib)
* [0xd4d](https://github.com/0xd4d) for creating [de4dot](https://github.com/0xd4d/de4dot)
* [Rottweiler](https://github.com/Rottweiler) for the PRs and help!
* [0megaD](https://github.com/0megaD) for the fixes which my eyes missed and for using dnpatch in his projects!
* [DivideREiS](https://github.com/dividereis) for fixing my typos and getting my lazy ass back to work on the BuildMemberRef/BuildCall method!
