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