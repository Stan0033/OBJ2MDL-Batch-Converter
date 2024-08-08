This program will convert dropped .obj (wavefront) files onto it, into .mdl (warcraft 3) and put them in the same folder as the source.

Notice:
   1. The output will always be a single geoset regardless of objects count.
   2. The output geoset might be sideways and with lower scaling but that can be fixed post-conversion.
     
Bonus to converted mdl file:
- origin ref
- geoset attached to bone "base", and using material
- material with layer using texture "Textures\white.blp"

Usage:

This software is free to use for personal and commercial
purposes, but modification of the source code is not allowed.
License:

This software is provided "as-is" without any warranty. The
author is not liable for any damages resulting from its use.
Redistribution is allowed, provided the software is unmodified.

changelog:
1.0.1:
- validating obj file format
- datetime not using local format anymore

1.0.2:
  - checking if vertices or faces count is invalid


Requires .net 3.5.

Single .exe file.

![alt text](https://i.ibb.co/CBPrxzN/Screenshot-2024-06-18-052810.png)
