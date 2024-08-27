This program will convert dropped .obj (wavefront) files onto it, into .mdl (warcraft 3) and put them in the same folder as the source.

Notice: If faces have to be triangulates from quads or ngons, the normals are not recalculated. Recaltulate them post-conversion.
     
Bonus to converted mdl file:
- origin ref
- geoset/s attached to bone/s, and using material with layer using texture "Textures\white.blp"
- stand and death sequence
  
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

1.0.3:
 - code optimizations
 - exponent is now recognized as valid by the validator

1.0.4:
- you can now choose to generate geoset for each object, bone for each geoset and material for each geoset


Requires .net 3.5.

Single .exe file.

![alt text](https://i.ibb.co/qM61dzr/Screenshot-2024-08-27-221956.png)
